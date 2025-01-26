using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;

namespace LoxStatEdit
{
    public partial class LoxStatFileForm: Form
    {
        const int _valueColumnOffset = 2;
        bool _inProgress;
        readonly string[] _args;
        LoxStatFile _loxStatFile;
        IList<LoxStatProblem> _problems;

        public LoxStatFileForm(params string[] args)
        {
            _args = args;
            InitializeComponent();

            // Subscribe to the MouseClick event of the chart
            _chart.MouseClick += _chartMouseClick;

            // Subscribe to the MouseMove event
            _dataGridView.MouseMove += DataGridView_MouseMove;
        }

        private void LoadFile()
        {
            _fileNameTextBox.Text = Path.GetFullPath(_fileNameTextBox.Text);
            _loxStatFile = LoxStatFile.Load(_fileNameTextBox.Text);
            _dataGridView.RowCount = 0;
            var columns = _dataGridView.Columns;
            while(columns.Count > (_valueColumnOffset + 1))
                columns.RemoveAt(columns.Count - 1);
            _fileInfoTextBox.Text = string.Format("{0}: {1} data point{2} with {3} value{4}",
                            _loxStatFile.Text, 
                            _loxStatFile.DataPoints.Count, 
                            _loxStatFile.DataPoints.Count == 1 ? "" : "s", 
                            _loxStatFile.ValueCount, 
                            _loxStatFile.ValueCount == 1 ? "" : "s");
            for(int i = 2; i <= _loxStatFile.ValueCount; i++)
            {
                var newColumn = (DataGridViewColumn)columns[_valueColumnOffset].Clone();
                newColumn.HeaderText = string.Format("{0} {1}", newColumn.HeaderText, i);
                columns.Add(newColumn);
            }
            _dataGridView.RowCount = _loxStatFile.DataPoints.Count;
            _dataGridView.MultiSelect = true;

            RefreshProblems();
            RefreshChart();
        }

        private void RefreshProblems()
        {
            _problems = LoxStatProblem.GetProblems(_loxStatFile);
            _problemButton.Enabled = _problems.Any();
        }

        private void RefreshChart()
        {
            // Now we can set up the Chart:
            List<Color> colors = new List<Color> { Color.Green, Color.Red, Color.Black, Color.Blue, Color.Violet, Color.Turquoise, Color.YellowGreen, Color.AliceBlue, Color.Beige, Color.Chocolate, Color.CornflowerBlue, Color.Firebrick };

            _chart.Series.Clear();

            int skipHeaderColumns = 2;

            for (int i = skipHeaderColumns; i < _dataGridView.Columns.Count; i++)
            {
                Series S = _chart.Series.Add(_dataGridView.Columns[i].HeaderText);
                S.ChartType = SeriesChartType.Spline;
                S.Color = colors[i];
            }


            // and fill in all the values from the dgv to the chart:
            for (int i = 0; i < _dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < _chart.Series.Count; j++)
                {
                    int p = _chart.Series[j].Points.AddXY(_dataGridView[1, i].Value, _dataGridView[j+ skipHeaderColumns, i].Value);
                }
            }
        }

        private void ShowProblems()
        {
            MessageBox.Show
            (
                this,
                string.Format
                (
                    "{1} Problems (showing max 50):{0}{2}",
                    Environment.NewLine,
                    _problems.Count,
                    string.Join(Environment.NewLine, _problems.Take(50))
                ),
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = _fileNameTextBox.Text;
            openFileDialog.InitialDirectory = Path.GetDirectoryName(_fileNameTextBox.Text);
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                _fileNameTextBox.Text = openFileDialog.FileName;

            LoadFile();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_inProgress) return;
            try
            {
                var rowIndex = e.RowIndex;
                var dataPoint = _loxStatFile.DataPoints[rowIndex];
                var columnIndex = e.ColumnIndex;
                e.Value = (columnIndex == 0) ? (object)dataPoint.Index :
                    (columnIndex == 1) ? (object)dataPoint.Timestamp :
                    (object)dataPoint.Values[e.ColumnIndex - _valueColumnOffset];
            }
            catch
            {
                e.Value = null;
            }
        }

        private void DataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var columnIndex = e.ColumnIndex;
            var dataPoint = _loxStatFile.DataPoints[e.RowIndex];
            if(columnIndex == 1)
                dataPoint.Timestamp = Convert.ToDateTime(e.Value);
            else if(columnIndex > 1)
                dataPoint.Values[columnIndex - _valueColumnOffset] =
                    Convert.ToDouble(e.Value.ToString());
            RefreshProblems();
            RefreshChart();
        }

        private void DataGridView_MouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Calculate selected", modalCalcSelected_Click));
                m.MenuItems.Add(new MenuItem("Calculate downwards", modalCalcFrom_Click));
                m.MenuItems.Add(new MenuItem("Insert entry above", modalInsertEntry_Click));
                m.MenuItems.Add(new MenuItem("Delete selected entries", modalDeleteSelected_Click));
                m.MenuItems.Add(new MenuItem("Fill and fix entries", modalFillEntries_Click));

                bool allowCalcSelected = true;
                bool allowCalcFrom = true;
                bool allowInsertEntry = true;
                bool allowDeleteSelected = true;

                foreach (DataGridViewCell cell in _dataGridView.SelectedCells)
                {
                    // check if "calculate selected" is allowed
                    // at least one row or calculable cell has to be selected
                    if (_dataGridView.SelectedRows.Count == 0 && cell.ColumnIndex < 2)
                    {
                        // At least one cell from the second row onwards has to be selected
                        allowCalcSelected = false;
                    }
                    //Console.WriteLine($"row: {cell.RowIndex}, column: {cell.ColumnIndex} is selected.");
                }
                //Console.WriteLine($"Selected rows: {_dataGridView.SelectedRows.Count}");
                //Console.WriteLine("---");

                // check if "calculate downwards" is allowed
                if (
                    (_dataGridView.SelectedRows.Count != 1 && _dataGridView.SelectedCells[0].ColumnIndex == 0)
                    ||
                    (_dataGridView.SelectedRows.Count == 0 && _dataGridView.SelectedCells.Count != 1)
                    ||
                    (_dataGridView.SelectedRows.Count == 0 && _dataGridView.SelectedCells[0].ColumnIndex < 2)
                )
                {
                    allowCalcFrom = false;
                }

                // check if "insert entry" is allowed
                if (_dataGridView.SelectedRows.Count != 1)
                {
                    allowInsertEntry = false;
                }

                // check if "delete selected entries" is allowed
                if (_dataGridView.SelectedRows.Count < 1)
                {
                    allowDeleteSelected = false;
                }


                // Calculate selected
                m.MenuItems[0].Enabled = allowCalcSelected;
                // Calculate downwards from this row
                m.MenuItems[1].Enabled = allowCalcFrom;
                // Insert entry
                m.MenuItems[2].Enabled = allowInsertEntry;
                // Delete selected entries
                m.MenuItems[3].Enabled = allowDeleteSelected;

                // Show the context menu on mouse position
                m.Show(_dataGridView, new Point(_dataGridView.PointToClient(Control.MousePosition).X, _dataGridView.PointToClient(Control.MousePosition).Y));

            }
        }

        private void modalCalcSelected_Click(object sender, EventArgs e)
        {
            string chrDec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string chrGrp = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string input = "v - 100";
            string pattern = "v";
            string replacement = "1";
            double myValue;
            string formula = "";
            bool isFormula = true;

            // Show the input dialog and check the result
            DialogResult dialogResult = ShowInputDialog(ref input, this);
            if (dialogResult == DialogResult.Cancel)
            {
                // User clicked "Cancel" or pressed "ESC", abort the operation
                Console.WriteLine("User cancelled the operation.");
                return;
            }


            // Replace comma, if it is the decimal separator
            if (chrDec == ",")
            {
                input = Regex.Replace(input, "/.", "");
                input = Regex.Replace(input, chrDec, chrGrp);
            }

            // Check if the input is a formula
            formula = Regex.Replace(input, pattern, replacement);
            try
            {
                myValue = Convert.ToDouble(new System.Data.DataTable().Compute(formula, null));
            }
            catch (System.Data.DataException de)
            {
                MessageBox.Show(de.Message);
                isFormula = false;
            }

            if (isFormula)
            {
                // Check if at least one cell is selected
                if (_dataGridView.SelectedCells.Count > 0)
                {
                    // Show the loading form
                    var busyForm = new BusyForm();
                    // Manually set the start position
                    busyForm.StartPosition = FormStartPosition.Manual;
                    var parent = this; // Reference to the parent form
                    busyForm.Left = parent.Left + (parent.Width - busyForm.Width) / 2;
                    busyForm.Top = parent.Top + (parent.Height - busyForm.Height) / 2;
                    busyForm.Show(this); // Or busyForm.ShowDialog(this) for a modal form
                    Application.DoEvents(); // Process events to ensure the loading form is displayed

                    try
                    {
                        foreach (DataGridViewCell cell in _dataGridView.SelectedCells)
                        {
                            int x = cell.RowIndex;
                            int y = cell.ColumnIndex;

                            // if a row is selected, skip first two columns
                            if (y >= 2)
                            {
                                replacement = Convert.ToDouble(_dataGridView.Rows[x].Cells[y].Value).ToString("#.###", CultureInfo.CreateSpecificCulture("en-EN"));
                                formula = Regex.Replace(input, pattern, replacement);
                                try
                                {
                                    myValue = Convert.ToDouble(new System.Data.DataTable().Compute(formula, null));
                                }
                                catch (System.Data.DataException)
                                {
                                    myValue = Convert.ToDouble(replacement);
                                }

                                _dataGridView.Rows[x].Cells[y].Value = myValue.ToString();
                            }
                        }
                    }
                    finally
                    {
                        // Close the loading form after the loop
                        busyForm.Close();
                    }

                }
                else
                {
                    MessageBox.Show("Something did not match. Please try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void modalCalcFrom_Click(object sender, EventArgs e)
        {
            string chrDec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string chrGrp = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string input = "v - 100";
            string pattern = "v";
            string replacement = "1";
            double myValue;
            string formula = "";
            bool isFormula = true;

            // Show the input dialog and check the result
            DialogResult dialogResult = ShowInputDialog(ref input, this);
            if (dialogResult == DialogResult.Cancel)
            {
                // User clicked "Cancel" or pressed "ESC", abort the operation
                Console.WriteLine("User cancelled the operation.");
                return;
            }

            // Replace comma, if it is the decimal separator
            if (chrDec == ",")
            {
                input = Regex.Replace(input, "/.", "");
                input = Regex.Replace(input, chrDec, chrGrp);
            }

            formula = Regex.Replace(input, pattern, replacement);
            try
            {
                myValue = Convert.ToDouble(new System.Data.DataTable().Compute(formula, null));
            }
            catch (System.Data.DataException de)
            {
                MessageBox.Show(de.Message);
                isFormula = false;
            }

            if (isFormula)
            {
                // Check if at least one row is selected
                if (_dataGridView.SelectedRows.Count == 1 || _dataGridView.SelectedCells.Count == 1)
                {
                    // Show the loading form
                    var busyForm = new BusyForm();
                    // Manually set the start position
                    busyForm.StartPosition = FormStartPosition.Manual;
                    var parent = this; // Reference to the parent form
                    busyForm.Left = parent.Left + (parent.Width - busyForm.Width) / 2;
                    busyForm.Top = parent.Top + (parent.Height - busyForm.Height) / 2;
                    busyForm.Show(this); // Or busyForm.ShowDialog(this) for a modal form
                    Application.DoEvents(); // Process events to ensure the loading form is displayed

                    try
                    {
                        int y_max;

                        // Check if all columns need to be recalculated or only one
                        if (_dataGridView.SelectedRows.Count == 1)
                        {
                            y_max = _dataGridView.ColumnCount;
                        }
                        else
                        {
                            y_max = _dataGridView.SelectedCells[0].ColumnIndex + 1;
                        }

                        for (int x = _dataGridView.SelectedCells[0].RowIndex; x < _dataGridView.Rows.Count; x++)
                        {

                            for (int y = _dataGridView.SelectedCells[0].ColumnIndex; y < y_max; y++)
                            {
                                //Console.WriteLine($"x: {x}, y: {y}");

                                // if a row is selected, skip first two columns
                                if (y >= 2)
                                {
                                    replacement = Convert.ToDouble(_dataGridView.Rows[x].Cells[y].Value).ToString("#.###", CultureInfo.CreateSpecificCulture("en-EN"));
                                    formula = Regex.Replace(input, pattern, replacement);
                                    try
                                    {
                                        myValue = Convert.ToDouble(new System.Data.DataTable().Compute(formula, null));
                                    }
                                    catch (System.Data.DataException)
                                    {
                                        myValue = Convert.ToDouble(replacement);
                                    }

                                    _dataGridView.Rows[x].Cells[y].Value = myValue.ToString();
                                }
                            }
                        }
                    }
                    finally
                    {
                        // Close the loading form after the loop
                        busyForm.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Something did not match. Please try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void modalInsertEntry_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            DataGridViewRow newRow = (DataGridViewRow)_dataGridView.Rows[0].Clone();
            DataGridViewRow atInsert = _dataGridView.SelectedRows[0];
            LoxStatDataPoint beforeDP = null; // Initialize beforeDP to null
            LoxStatDataPoint newDP;
            LoxStatDataPoint atDP = _loxStatFile.DataPoints[atInsert.Index];

            bool insertAllowed = false;

            if (atInsert.Index == 0)
            {
                insertAllowed = true; // Insertion at the beginning is always possible
            }
            else
            {
                // Ensure beforeDP is assigned before this point
                beforeDP = _loxStatFile.DataPoints[atDP.Index - 1];
                if (beforeDP != null && beforeDP.Timestamp < atDP.Timestamp.AddMinutes(-119)) // Check for null to satisfy the compiler
                {
                    insertAllowed = true; // Insertion is possible based on timestamp comparison
                }
                else
                {
                    MessageBox.Show("There is no place to insert an entry!\n\nPlease check again, if there is a missing entry.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Only adjust indices and insert new data point if insertion is confirmed
            if (insertAllowed)
            {
                for (int x = atDP.Index; x < _loxStatFile.DataPoints.Count; x++)
                {
                    _loxStatFile.DataPoints[x].Index += 1;
                }

                if (atInsert.Index == 0)
                {
                    newDP = atDP.Clone();
                    newDP.Index = 0;
                    newDP.Values[0] = 0;
                    newDP.Timestamp = new DateTime(atDP.Timestamp.Year, atDP.Timestamp.Month, 1, 0, 0, 0);
                    _dataGridView.Rows.Insert(0, newRow);
                    _loxStatFile.DataPoints.Insert(0, newDP);
                }
                else
                {
                    // Insertion logic for non-first row already validated above
                    if (beforeDP != null) // Additional null check for safety
                    {
                        newDP = beforeDP.Clone();
                        newDP.Index = atInsert.Index;
                        newDP.Timestamp = beforeDP.Timestamp.AddHours(1);
                        _dataGridView.Rows.Insert(atInsert.Index - 1, newRow);
                        _loxStatFile.DataPoints.Insert(atInsert.Index - 1, newDP);
                    }
                }
            }

            Cursor = Cursors.Default;
        }

        private void modalDeleteSelected_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            int tmpID;
            //_inProgress = true;
            int rowIndex = _dataGridView.SelectedRows[0].Index;
            int selCount = _dataGridView.SelectedRows.Count;
            int lastSelIndex = _dataGridView.SelectedRows[_dataGridView.SelectedRows.Count - 1].Index;
            //_loxStatFile.DataPoints.RemoveAt(rowIndex);
            for (int x = 0; x < selCount; x++)
            {
                _loxStatFile.DataPoints.RemoveAt(_dataGridView.SelectedRows[0].Index);
                _dataGridView.Rows.RemoveAt(_dataGridView.SelectedRows[0].Index);
            }
            //_dataGridView.Rows.Remove(_dataGridView.SelectedRows[0]);
            int rowCount = _dataGridView.RowCount;
            for (int x = lastSelIndex; x < rowCount; x++)
            {
                if (x == 0)
                {
                    _loxStatFile.DataPoints[x].Index = 0;
                   // _dataGridView.Rows[x].Cells[0].Value = 0;
                }
                else
                {
                    tmpID = _loxStatFile.DataPoints[x - 1].Index + 1;
                    _loxStatFile.DataPoints[x].Index = tmpID;
                   // _dataGridView.Rows[x].Cells[0].Value = ((int)_dataGridView.Rows[x - 1].Cells[0].Value + 1).ToString();
                }
            }
            _dataGridView.Refresh();
            RefreshProblems();
            RefreshChart();
            _inProgress = false;
            Cursor = Cursors.Default;
        }

        private void modalFillEntries_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            // Sort list by timestamp, remove duplicates
            _loxStatFile.DataPoints = _loxStatFile.DataPoints
                .OrderBy(dp => dp.Timestamp)  // Sort by timestamp
                .GroupBy(dp => dp.Timestamp)  // Group by timestamp
                .Select(g => g.First())  // Remove duplicated timestamps
                .Where(dp =>
                    dp.Index >= 0 &&  // Ensure Index is set
                    dp.Timestamp != null  // Ensure Timestamp is set
                )
                .Select((dp, index) => { dp.Index = index; return dp; })  // Re-index
                .ToList();

            // Update DataGridView row count, else it might by that there are empty rows shown
            _dataGridView.RowCount = _loxStatFile.DataPoints.Count;

            for (int i = 0; i < _loxStatFile.DataPoints.Count - 1; i++)
            {
                LoxStatDataPoint currentDP = _loxStatFile.DataPoints[i];
                LoxStatDataPoint nextDP = _loxStatFile.DataPoints[i + 1];
                TimeSpan ts = nextDP.Timestamp - currentDP.Timestamp;
                double hoursDiff = ts.TotalHours;

                // Check if there's more than 1 hour difference indicating a gap
                if (hoursDiff > 1)
                {
                    double insertCount = hoursDiff - 1;
                    double diff = (nextDP.Values[0] - currentDP.Values[0]) / (insertCount + 1);

                    // Adjust indices for subsequent data points
                    for (int x = nextDP.Index; x < _loxStatFile.DataPoints.Count; x++)
                    {
                        _loxStatFile.DataPoints[x].Index += (int)insertCount;
                    }

                    // Insert missing data points
                    for (int x = 1; x <= insertCount; x++)
                    {
                        LoxStatDataPoint newDP = currentDP.Clone();
                        newDP.Index += x;
                        newDP.Timestamp = newDP.Timestamp.AddHours(x);
                        newDP.Values[0] += diff * x;
                        _loxStatFile.DataPoints.Insert(newDP.Index, newDP);
                        _dataGridView.Rows.Insert(newDP.Index, 1);
                    }

                    // Adjust loop counter to skip newly inserted points
                    i += (int)insertCount;
                }
            }

            _dataGridView.Refresh();
            RefreshProblems();
            RefreshChart();
            Cursor = Cursors.Default;
        }

        private static DialogResult ShowInputDialog(ref string input, Form parentForm)
        {
            System.Drawing.Size size = new System.Drawing.Size(250, 130);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Name";
            inputBox.StartPosition = FormStartPosition.CenterParent; // Set the start position

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 20, 23);
            textBox.Location = new System.Drawing.Point(10, 10);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Label lblInfo = new Label();
            lblInfo.Size = new Size(size.Width - 30, 60);
            lblInfo.Location = new Point(15, 37);
            lblInfo.Text = "Insert a formula for calculating the values. Allowed operators: + - / * ( )\n" +
                "Use v for the original value.\n" +
                "Like: v - 100";
            inputBox.Controls.Add(lblInfo);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 99);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 99);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
			// if not remote desktop session then enable double-buffering optimization
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                typeof(DataGridView).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, _dataGridView, new object[] { true });
            }

            if((_args != null) && (_args.Length > 0))
            {
                _fileNameTextBox.Text = _args[0];
                LoadFile();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            RefreshProblems();
            if(_problems.Any())
                ShowProblems();
            if(MessageBox.Show(
                this,
                "### DISCLAIMER ###\n" + 
                "- I will not (and can not) take any responsibility for any\n" + 
                "  issues of your Loxone installation whatsoever!\n" +
                "- Loxone will most likely not support any issues, if you use the\n" +
                "  files generated by this tool either!\n" +
                "- I am just a desparate Loxone user in need of a solution for a\n" +
                "  problem and lucky enough to be a professional coder!\n" +
                "- Loxone did not provide me a full file format documentation\n" +
                "  and I used Hexinator (thank you!) to decode the file format\n" + 
                "  to some degree!\n\n" +
                "### NOTICE ###\n" +
                "To apply the modified statistics further steps are needed!\n" +
                "1. Upload the modified statistics\n" +
                "2. Restart the Miniserver\n" + "" +
                "3. Clear the app cache\n" + 
                "If you don't know how to clear the app cache, then remove\n" +
                "and re-add the Miniserver in the app. Should this not help\n" + "" +
                "then uninstall and install the app again.\n\n" +
                "Would you still like to save the file?",
                "DISCLAIMER! USE AT YOUR OWN RISK!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
                ) != DialogResult.Yes)
                return;
            _loxStatFile.FileName = Path.GetFullPath(_fileNameTextBox.Text);
            _loxStatFile.Save();
            MessageBox.Show(this, "Save successful!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProblemButton_Click(object sender, EventArgs e)
        {
            ShowProblems();
        }

        private void _dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void _dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                string CopiedContent = Clipboard.GetText();
                string[] Lines = CopiedContent.Split('\n');
                int StartingRow = _dataGridView.CurrentCell.RowIndex;
                int StartingColumn = _dataGridView.CurrentCell.ColumnIndex;
                foreach (var line in Lines)
                {
                    if (StartingRow <= (_dataGridView.Rows.Count - 1))
                    {
                        string[] cells = line.Split('\t');
                        int ColumnIndex = StartingColumn;
                        for (int i = 0; i < cells.Length && ColumnIndex <= (_dataGridView.Columns.Count - 1); i++, ColumnIndex++)
                        {
                            if (!String.IsNullOrEmpty(cells[i]))
                            {
                                if (ColumnIndex == 0) // Specific check for column with index 0
                                {
                                    // Do nothing, as the index is not editable
                                }
                                else if (ColumnIndex == 1) // Specific check for column with index 1
                                {
                                    var input = cells[i].Trim();

                                    // Attempt to parse the cell value as a DateTime
                                    DateTime valueDateTime;
                                    string[] formats = { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm" };
                                    // bool isDateTime = DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out valueDateTime);
                                    bool isDateTime = DateTime.TryParseExact(input, formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out valueDateTime);

                                    if (isDateTime)
                                    {
                                        // If the value is a valid DateTime, assign it
                                        var dataPoint = _loxStatFile.DataPoints[StartingRow];
                                        //dataPoint.Values[ColumnIndex - _valueColumnOffset] = valueDateTime;
                                        //Console.WriteLine($"StartingRow: {StartingRow} - StartingColumn: {StartingColumn} - i: {i} - ColumnIndex: {ColumnIndex}");

                                        dataPoint.Timestamp = valueDateTime;
                                    }
                                    else
                                    {
                                        // Handle parsing failure, e.g., by showing an error message or skipping the assignment
                                        MessageBox.Show($"The value '{input}' is not a valid date time. Please use 'dd.MM.yyyy HH:mm:ss'.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                                else // Existing check for other columns
                                {
                                    string chrDec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                                    string chrGrp = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

                                    var input = cells[i].Trim();

                                    // Replace comma, if it is the decimal separator
                                    // Is this really needed? Have to test with different locales
                                    /*if (chrDec == ",")
                                    {
                                        input = Regex.Replace(input, "/.", "");
                                        input = Regex.Replace(input, chrDec, chrGrp);
                                    }*/

                                    // Attempt to parse the cell value as a double
                                    double valueDouble;
                                    bool isDouble = double.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out valueDouble);

                                    if (isDouble)
                                    {
                                        // If the value is a valid double, assign it
                                        var dataPoint = _loxStatFile.DataPoints[StartingRow];
                                        dataPoint.Values[ColumnIndex - _valueColumnOffset] = valueDouble;
                                    }
                                    else
                                    {
                                        // If the value is not a valid double, show an error message
                                        MessageBox.Show($"The value '{input}' is not a valid number.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                        }
                        StartingRow++;
                    }
                }
                RefreshProblems();
                RefreshChart();
                _dataGridView.Refresh();
            }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            int distance = 5; // Set this to the desired distance between _chart and _dataGridView
            int availableWidth = this.ClientSize.Width - distance - 23; // window width - distance - padding

            _dataGridView.Width = (int)(availableWidth * 0.45);
            _chart.Left = _dataGridView.Right + distance;
            _chart.Width = (int)(availableWidth * 0.55);
        }

        private void _chartMouseClick(object sender, MouseEventArgs e)
        {
            // Define a tolerance in pixels
            const double tolerance = 10;

            // Perform a hit test to find the clicked chart element
            HitTestResult result = _chart.HitTest(e.X, e.Y);

            // Initialize variables to track the closest data point and its distance
            double minDistance = double.MaxValue;
            int closestPointIndex = -1;

            // Check all series in the chart
            foreach (Series series in _chart.Series)
            {
                for (int i = 0; i < series.Points.Count; i++)
                {
                    DataPoint point = series.Points[i];
                    // Convert data point position to pixel position in the chart
                    double pointX = _chart.ChartAreas[0].AxisX.ValueToPixelPosition(point.XValue);
                    double pointY = _chart.ChartAreas[0].AxisY.ValueToPixelPosition(point.YValues[0]);

                    // Calculate distance from the click to this data point
                    double distance = Math.Sqrt(Math.Pow(e.X - pointX, 2) + Math.Pow(e.Y - pointY, 2));

                    // Check if this is the closest data point so far
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPointIndex = i;
                    }
                }
            }

            // If the closest data point is within the tolerance, treat it as a click on that point
            if (minDistance <= tolerance && closestPointIndex != -1)
            {
                // Assuming the closest data point index corresponds to the row index in the DataGridView
                // Make sure to validate the index before using it
                if (closestPointIndex >= 0 && closestPointIndex < _dataGridView.Rows.Count)
                {
                    // Clear the current selection
                    _dataGridView.ClearSelection();

                    // Select the corresponding row in the DataGridView
                    _dataGridView.Rows[closestPointIndex].Selected = true;

                    // Optionally, scroll to the selected row if it's not visible
                    _dataGridView.FirstDisplayedScrollingRowIndex = closestPointIndex;
                }
            }
        }

        private void DataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            // Perform a hit test to find the column index under the mouse
            var hitTestInfo = _dataGridView.HitTest(e.X, e.Y);
            if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
            {
                // Check if the column under the mouse is read-only
                bool isReadOnly = _dataGridView.Columns[hitTestInfo.ColumnIndex].ReadOnly;
                if (isReadOnly)
                {
                    // Change the cursor to indicate the column is read-only
                    _dataGridView.Cursor = Cursors.No;
                }
                else
                {
                    // Reset the cursor to the default
                    _dataGridView.Cursor = Cursors.Default;
                }
            }
            else
            {
                // Reset the cursor to the default if not over a cell
                _dataGridView.Cursor = Cursors.Default;
            }
        }

    }
}
