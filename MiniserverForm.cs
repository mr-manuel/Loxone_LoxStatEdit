using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoxStatEdit
{
    public partial class MiniserverForm: Form
    {

        #region class FileItem
        class FileItem: IComparable<FileItem>
        {
            internal MsFileInfo MsFileInfo;
            internal FileInfo FileInfo;
            private string _name;

            public string FileName
            {
                get
                {
                    return (MsFileInfo != null) ? MsFileInfo.FileName : FileInfo.Name;
                }
            }
            public bool IsValidMsStatsFile
            {
                get
                {
                    var fileName = (MsFileInfo != null) ? MsFileInfo.FileName : FileInfo.Name;

                    // verify if file has valid Loxone stats format with UUID and .yyyyMM extension
                    var pattern = @"([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{16})(_[1-9])?\.([12][0-9]{3})([01][0-9])";
                    // 1st group (): UUID 
                    // 2nd group (): yyyy
                    // 3rd group (): MM 
                    var result = Regex.Match(fileName, pattern);

                    // no further evaluation of Regex so far

                    var validFormat = false;
                    if (result.Success)
                        validFormat = true;
                    
                    return (validFormat);
                }
            }

            public string Name
            {
                get
                {
                    if(_name == null)
                    {
                        try
                        {
                            if (FileInfo != null)
                            {
                                _name = LoxStatFile.Load(FileInfo.FullName, true).Text;
                                
                                // Append the file extension as a suffix could be _1.202407 or 202407
                                // This is a workaround until the sorting on two levels is implemented
                                // e.g. sort by description and then by filename, else the files are not in historical order
                                _name += " [";

                                // check if filename contains _
                                if (FileInfo.Name.Contains("_"))
                                {
                                    string[] split = FileInfo.Name.Split('_');
                                    _name += split[1];
                                }
                                else { 
                                    _name += FileInfo.Name.Substring(FileInfo.Name.Length - 6);
                                }

                                _name += "]";
                            }
                        }
                        catch(Exception)
                        {
                            _name = null;
                        }
                        if(string.IsNullOrEmpty(_name))
                            //_name = FileName;
                            _name = "Download to view description";
                    }
                    return _name;
                }
            }

            public DateTime YearMonth
            {
                get
                {
                    return LoxStatFile.GetYearMonth(FileName);
                }
            }

            public DateTime DateModified
            {
                get
                {
                    return (FileInfo != null) ? FileInfo.LastWriteTime : MsFileInfo.Date;
                }
            }

            public long Size
            {
                get
                {
                    return (FileInfo != null) ? FileInfo.Length : MsFileInfo.Size;
                }
            }

            public const int IDStatusOnlyOnMS = 1;

            public const int IDStatusOnlyOnFS = 2;

            public const int IDStatusNewerOnMS = 3;

            public const int IDStatusNewerOnFS = 4;

            public const int IDStatusLargerOnMS = 5;

            public const int IDStatusLargerOnFS = 6;

            public const int IDStatusSame = 7;
            public int Status
            {
                get
                {
                    if (FileInfo == null)
                    {
                        return IDStatusOnlyOnMS;
                    }
                    if (MsFileInfo == null)
                    {
                        return IDStatusOnlyOnFS;
                    }
                    if (FileInfo.LastWriteTime < MsFileInfo.Date)
                    {
                        return IDStatusNewerOnMS;
                    }
                    if (FileInfo.LastWriteTime > MsFileInfo.Date)
                    {
                        return IDStatusNewerOnFS;
                    }
                    if (FileInfo.Length < MsFileInfo.Size)
                    {
                        return IDStatusLargerOnMS;
                    }
                    if (FileInfo.Length > MsFileInfo.Size)
                    {
                        return IDStatusLargerOnFS;
                    }
                    return IDStatusSame;
                }
            }

            public string StatusString
            {
                get
                {
                    string statusString;

                    switch (Status)
                    {
                        case IDStatusOnlyOnMS:
                            statusString = "Only on MS";
                            break;
                        case IDStatusOnlyOnFS:
                            statusString = "Only on FS";
                            break;
                        case IDStatusNewerOnMS:
                            statusString = "Newer on MS";
                            break;
                        case IDStatusNewerOnFS:
                            statusString = "Older on MS";
                            break;
                        case IDStatusLargerOnMS:
                            statusString = "Larger on MS";
                            break;
                        case IDStatusLargerOnFS:
                            statusString = "Smaller on MS";
                            break;
                        default:
                            // IDStatusSame
                            statusString = "Same date/size";
                            break;
                    }
                    
                    return statusString;
                }
            }

            public override string ToString()
            {
                return string.Format("{0} ({1})", Name, StatusString);
            }

            public int CompareTo(FileItem other)
            {
                var c = StringComparer.OrdinalIgnoreCase.Compare(Name, other.Name);
                return (c != 0) ? c : YearMonth.CompareTo(other.YearMonth);
            }
        }

        #endregion

        #region Fields
        readonly string[] _args;
        IList<MsFileInfo> _msFolder = new MsFileInfo[0];
        IList<FileInfo> _localFolder = new FileInfo[0];
        IList<FileItem> _fileItems;

        #endregion

        #region Constructor
        public MiniserverForm(params string[] args)
        {
            _args = args;
            InitializeComponent();

            // Set the start position of the form to the center of the screen
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        #endregion

        #region Methods
        private void RefreshGridView()
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
                // provide a empty list to prevent crash
                if (_msFolder == null)
                {
                    _msFolder = new List<MsFileInfo>();
                }

                // Save the current sort conditions
                var sortColumn = _dataGridView.SortedColumn;
                var sortOrder = _dataGridView.SortOrder;

                // Save the current scrolling position
                int scrollPosition = _dataGridView.FirstDisplayedScrollingRowIndex;
                //Console.WriteLine($"Scroll position: {scrollPosition}");

                // Save the current selection
                var selectedCells = _dataGridView.SelectedCells.Cast<DataGridViewCell>()
                    .Select(cell => new { cell.RowIndex, cell.ColumnIndex })
                    .ToList();

                // Save the current active cell (for restoring the selection arrow)
                int? activeCellRowIndex = _dataGridView.CurrentCell?.RowIndex;
                int? activeCellColumnIndex = _dataGridView.CurrentCell?.ColumnIndex;

                var msMap = _msFolder.ToLookup(e => e.FileName, StringComparer.OrdinalIgnoreCase);
                var localMap = _localFolder.ToLookup(e => e.Name, StringComparer.OrdinalIgnoreCase);

                // Create a new SortableBindingList
                _fileItems = new SortableBindingList<FileItem>();

                // Retrieve the filter text
                string filterText = _filterTextBox.Text.ToLower();

                // Get the data
                var data = msMap.Select(e => e.Key).Union(localMap.Select(e => e.Key)).
                    Select(f => new FileItem
                    {
                        MsFileInfo = msMap[f].FirstOrDefault(),
                        FileInfo = localMap[f].FirstOrDefault(),
                    }).
                    // Order by filename if available else by name
                    Where(f => f.FileName.ToLower().Contains(filterText) || f.Name.ToLower().Contains(filterText)).
                    OrderBy(f => f.MsFileInfo?.FileName ?? f.FileInfo?.Name).
                    ToList();

                // Add the data to the SortableBindingList
                foreach (var item in data)
                {
                    _fileItems.Add(item);
                }

                // Disable automatic column generation
                _dataGridView.AutoGenerateColumns = false;

                // Clear existing columns
                _dataGridView.Columns.Clear();

				// Add Cell Formatting
                _dataGridView.CellFormatting += DataGridView_CellFormatting;
                
                // Add Tool Tips
                _dataGridView.CellToolTipTextNeeded += DataGridView_CellToolTipTextNeeded;

                // Add columns manually in the order you want
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                    DataPropertyName = "FileName",
                    HeaderText = "FileName",
                    Width = 250,
                    ToolTipText = "Name of the file on the Loxone MS and local file system.\n"+
                        "NOTE: This is typically an UUID plus extension with .yyyyMM",
                });
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                    DataPropertyName = "Name",
                    HeaderText = "Description",
                    MinimumWidth = 250,
                    ToolTipText = "Defined in Loxone Config and retrieved from statistics data.",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                });
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "YearMonth",
                    HeaderText = "Year/Month",
                    Width = 70,
                    ToolTipText = "Year and month from file extension on MS. \n"+
                        "NOTE: This date is used on MS to fetch statistics data from a specific year and month.",
                    DefaultCellStyle = { Format = "yyyy-MM" }
                });
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "DateModified",
                    HeaderText = "Date modified",
                    Width = 120,
                    ToolTipText = "Date when the file was modified. \nNOTE: files that were downloaded via FTP always " +
                                  "have a date with this or last year.",
                    DefaultCellStyle = { Format = "dd.MM.yyyy - HH:mm:ss" }
                });
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                    DataPropertyName = "Size",
                    HeaderText = "Size",
                    Width = 90,
                    ToolTipText = "Size of the file on the local file system or MS.",
                    DefaultCellStyle = new DataGridViewCellStyle()
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        ForeColor = System.Drawing.Color.Black
                    },
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                });
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                    DataPropertyName = "StatusString",
                    HeaderText = "Status",
                    Width = 100,
                    ToolTipText = "Result of a comparision between file on Loxone MS and local file system.",
                    DefaultCellStyle = new DataGridViewCellStyle()
                    {
                        BackColor = System.Drawing.Color.White,
                        ForeColor = System.Drawing.Color.Black
                    },
                });
                _dataGridView.Columns.Add(new DataGridViewButtonColumn {
                    DataPropertyName = "Download",
                    HeaderText = "Download",
                    Width = 60,
                    ToolTipText = "Copy file(s) from Loxone MS to local file system."
                });
                _dataGridView.Columns.Add(new DataGridViewButtonColumn {
                    DataPropertyName = "Edit",
                    HeaderText = "Edit",
                    Width = 50,
                    ToolTipText = "Edit statistical data (entries) of the file on local file system."
                });
                _dataGridView.Columns.Add(new DataGridViewButtonColumn {
                    DataPropertyName = "Upload",
                    HeaderText = "Upload",
                    Width = 60,
                    ToolTipText = "Copy file(s) from local file system to Loxone MS."
                });
                _dataGridView.Columns.Add(new DataGridViewButtonColumn {
                    DataPropertyName = "Convert",
                    HeaderText = "Convert",
                    Width = 60,
                    ToolTipText = "Convert data from old meter function block to new style."
                });
                _dataGridView.Columns.Add(new DataGridViewButtonColumn {
                    DataPropertyName = "Delete",
                    HeaderText = "Delete",
                    Width = 60,
                    ToolTipText = "Delete file on Loxone MS and local file system after confirmation."
                });

                // Bind the SortableBindingList to the DataGridView
                _dataGridView.DataSource = _fileItems;

                // Restore the sort conditions
                if (sortColumn != null && sortOrder != SortOrder.None)
                {
                    // Determine the ListSortDirection from the SortOrder
                    ListSortDirection direction = sortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;

                    // Find the DataGridViewColumn to sort by
                    DataGridViewColumn columnToSort = _dataGridView.Columns
                        .OfType<DataGridViewColumn>()
                        .FirstOrDefault(c => c.DataPropertyName == sortColumn.DataPropertyName);

                    // TODO: Add second sort level, to always sort by FileName as second level with same direction as first level

                    if (columnToSort != null)
                    {
                        // Reapply the sort
                        _dataGridView.Sort(columnToSort, direction);
                    }
                }

                // Restore the scrolling position
                if (_dataGridView.RowCount > 0 && scrollPosition != -1 && scrollPosition < _dataGridView.RowCount)
                {
                    _dataGridView.FirstDisplayedScrollingRowIndex = scrollPosition;
                }

                // Clear the default selection
                _dataGridView.ClearSelection();

                // Restore the CurrentCell to restore the selection arrow
                if (activeCellRowIndex.HasValue && activeCellColumnIndex.HasValue
                    && activeCellRowIndex.Value < _dataGridView.RowCount
                    && activeCellColumnIndex.Value < _dataGridView.ColumnCount)
                {
                    _dataGridView.CurrentCell = _dataGridView.Rows[activeCellRowIndex.Value].Cells[activeCellColumnIndex.Value];
                }

                // Restore the selection
                if (selectedCells.Any())
                {
                    foreach (var cell in selectedCells)
                    {
                        if (cell.RowIndex < _dataGridView.RowCount && cell.ColumnIndex < _dataGridView.ColumnCount)
                        {
                            _dataGridView.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Selected = true;
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

        private void RefreshMs()
        {
            var uriBuilder = GetMsUriBuilder();
            if (uriBuilder != null) {
                _msFolder = MsFileInfo.Load(uriBuilder.Uri);
            }
        }

        private void RefreshLocal()
        {
            Directory.CreateDirectory(_folderTextBox.Text);
            _localFolder = Directory.EnumerateFiles(_folderTextBox.Text).
                Select(fileName => new FileInfo(fileName)).ToList();
        }

        private void BrowseFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = _folderTextBox.Text;
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                _folderTextBox.Text = folderBrowserDialog.SelectedPath;
            RefreshLocal();
            RefreshGridView();
            //Console.WriteLine("Refreshed local folder");
        }

        private void openFolderButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", _folderTextBox.Text);
        }

        private UriBuilder GetMsUriBuilder()
        {
            try
            {
                // escape # in uri by replacing with hex
                var uriBuilder = new UriBuilder(new Uri(_urlTextBox.Text.Replace("#", "%23")).
                    GetComponents(UriComponents.Scheme | UriComponents.UserInfo |
                    UriComponents.Host | UriComponents.Port, UriFormat.UriEscaped));
                uriBuilder.Path = "stats";
                return uriBuilder;
            }
            catch (UriFormatException ex)
            {
                MessageBox.Show(ex.Message, "Error - UriBuilder", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }


        private Uri GetFileNameUri(string fileName)
        {
            var uriBuilder = GetMsUriBuilder();
            uriBuilder.Path += "/" + fileName;
            return uriBuilder.Uri;
        }

        private bool Download(FileItem fileItem)
        {
            try
            {
                var ftpWebRequest = (FtpWebRequest)FtpWebRequest.
                Create(GetFileNameUri(fileItem.FileName));
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                var targetFileName = Path.Combine(_folderTextBox.Text, fileItem.FileName);
                using(var response = ftpWebRequest.GetResponse())
                using(var ftpStream = response.GetResponseStream())
                using(var fileStream = File.OpenWrite(targetFileName))
                ftpStream.CopyTo(fileStream);                
                File.SetLastWriteTime(targetFileName, fileItem.MsFileInfo.Date);

                // Log FTP output
                // File.AppendAllText("./custom.log", $"Downloaded {fileItem.FileName} - Setting last write time to {fileItem.MsFileInfo.Date}\n\n");

                return true;
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                {
                    MessageBox.Show(ex.Message, "Error  - FTP connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"# Message\n{ex.Message}\n\n# Data\n{ex.Data}\n\n# StackTrace\n{ex.StackTrace}", 
                    "Error - IList", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        private bool Upload(FileItem fileItem)
        {
            try
            {
                var ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(GetFileNameUri(fileItem.FileName));
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                using (var fileStream = File.OpenRead(
                    Path.Combine(_folderTextBox.Text, fileItem.FileInfo.FullName)))
                using (var ftpStream = ftpWebRequest.GetRequestStream())
                    fileStream.CopyTo(ftpStream);
                using (var response = ftpWebRequest.GetResponse()) { }

                return true;
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                {
                    MessageBox.Show(ex.Message, "Error  - FTP connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"# Message\n{ex.Message}\n\n# Data\n{ex.Data}\n\n# StackTrace\n{ex.StackTrace}", 
                    "Error - IList", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        private bool Convert(FileItem fileItem)
        {
            // ToDo: Code missing
            System.Threading.Thread.Sleep(50);
            return true;
        }

        private bool Delete(FileItem fileItem)
        {
            try
            {
                // Delete file on Loxone MS
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(GetFileNameUri(fileItem.FileName));
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                //Console.WriteLine("Delete status: {0}", ftpWebResponse.StatusDescription);
                ftpWebResponse.Close();

                // Delete file on local file system
                var fileNameWithPath = Path.Combine(_folderTextBox.Text, fileItem.FileName);
                if (File.Exists(fileNameWithPath))
                {
                    File.Delete(fileNameWithPath);
                }

                return true;
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                {
                    MessageBox.Show(ex.Message, "Error  - FTP connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"# Message\n{ex.Message}\n\n# Data\n{ex.Data}\n\n# StackTrace\n{ex.StackTrace}", 
                    "Error - local file system", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }
        #endregion

        #region Events
        private void RefreshMsButton_Click(object sender, EventArgs e)
        {
            RefreshMs();
            RefreshGridView();
        }

        private void RefreshFolderButton_Click(object sender, EventArgs e)
        {
            RefreshLocal();
            RefreshGridView();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RefreshGridView();
        }

        private void _urlTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(_folderTextBox.Text))
            {
                RefreshMsButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void _folderTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(_folderTextBox.Text))
            {
                RefreshFolderButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void _filterTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(_folderTextBox.Text))
            {
                RefreshGridView();
            }
        }

        private void MiniserverForm_Load(object sender, EventArgs e)
        {

            // if not remote desktop session then enable double-buffering optimization
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                typeof(DataGridView).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, _dataGridView, new object[] { true });
            }

            _folderTextBox.Text = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments), "LoxStatEdit");
            if(_args.Length >= 1)
            {
                _urlTextBox.Text = _args[0];
                RefreshMs();
            }
            if(_args.Length >= 2)
                _folderTextBox.Text = Path.GetFullPath(_args[1]);
            RefreshLocal();
            RefreshGridView();
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= _fileItems.Count))
            {
                e.Value = null;
                return;
            }
            if (e.ColumnIndex == 0) // The FileName column is at index 0
            {
                var fileItem = _fileItems[e.RowIndex];
                if (!fileItem.IsValidMsStatsFile)
                {
                    e.CellStyle.BackColor = System.Drawing.Color.Red;
                }
            }
            if (e.ColumnIndex == 1) // The Description / Name column is at index 1
            {
                var fileItem = _fileItems[e.RowIndex];
                if (fileItem.Name == "Download to view description")
                {
                    e.CellStyle.Font = new System.Drawing.Font(e.CellStyle.Font, System.Drawing.FontStyle.Italic);
                }
                else
                {
                    e.CellStyle.Font = new System.Drawing.Font(e.CellStyle.Font, System.Drawing.FontStyle.Regular);
                }
            }
            if (e.ColumnIndex == 5) // The Status column is at index 5
            {
                var fileItem = _fileItems[e.RowIndex];
                switch (fileItem.Status)
                {
                    case FileItem.IDStatusOnlyOnMS:
                        e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                        break;
                    case FileItem.IDStatusOnlyOnFS:
                        e.CellStyle.BackColor = System.Drawing.Color.LightGoldenrodYellow;
                        break;
                    case FileItem.IDStatusNewerOnMS:
                        e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
                        break;
                    case FileItem.IDStatusNewerOnFS:
                        e.CellStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                        break;
                    case FileItem.IDStatusLargerOnMS:
                        e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
                        break;
                    case FileItem.IDStatusLargerOnFS:
                        e.CellStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                        break;
                    default:
                        // IDStatusSame:
                        e.CellStyle.BackColor = System.Drawing.Color.White;
                        break;
                }
            }

        }
        
        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                if ((e.RowIndex < 0) || (e.RowIndex >= _fileItems.Count))
                {
                    e.Value = null;
                    return;
                }
                var fileItem = _fileItems[e.RowIndex];
                switch(e.ColumnIndex)
                {
                    case 0: e.Value = fileItem.FileName; break;
                    case 1: e.Value = fileItem.Name; break;
                    case 2: e.Value = fileItem.YearMonth; break;
                    case 3: e.Value = fileItem.DateModified; break;
                    case 4: e.Value = fileItem.Size; break;
                    case 5: e.Value = fileItem.StatusString; break;
                    case 6: e.Value = "Download"; break;
                    case 7: e.Value = "Edit"; break;
                    case 8: e.Value = "Upload"; break;
                    case 9: e.Value = "Convert"; break;
                    case 10: e.Value = "Delete"; break;
                    default: e.Value = null; break;
                }
            }
            catch
            {
            #if DEBUG
                Debugger.Break();
            #endif
                e.Value = null;
            }
        }

        private void DataGridView_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            try
            {
                if ((e.RowIndex < 0) || (e.RowIndex >= _fileItems.Count))
                {
                    e.ToolTipText = null;
                    return;
                }
               
                var fileItem = _fileItems[e.RowIndex];
                switch (e.ColumnIndex)
                {
                    case 0: // fileItem.FileName; 
                        if (!fileItem.IsValidMsStatsFile)
                        {
                            e.ToolTipText = "The UUID and/or file extension looks malformed.\n\n "+
                                "Please check, if it is used by the MS. If not, you may consider to remove the file.";
                        }
                        break;
                    case 1: // fileItem.Name; 
                        break;
                    case 2: // fileItem.YearMonth; 
                        break;
                    case 3: // fileItem.DateModified; 
                        break;
                    case 4: // fileItem.Size; 
                        break;
                    case 5: // fileItem.StatusString; 
                        break;
                    default:
                        break;
                }
            }
            catch
            {
#if DEBUG
                Debugger.Break();
#endif
                e.ToolTipText = "";
            }
        }

        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; //When clicking the header row
            var fileItem = _fileItems[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 6: //Download
                    progressBar.Maximum = 1;
                    progressBar.Value = 0;
                    progressBar.SetState(1);
                    progressLabel.Text = "Starting download...";
                    if(Download(fileItem))
                    {
                        progressBar.Value = 1;
                        progressLabel.Text = "Download complete!";
                        RefreshLocal();
                        RefreshGridView();
                    }
                    else
                    {
                        progressBar.Value = 1;
                        progressBar.SetState(2);
                        progressLabel.Text = "Download failed!";
                    }
                    break;
                case 7: //Edit
                    // show error message, if file is not downloaded yet
                    if (fileItem.FileInfo == null)
                    {
                        MessageBox.Show($"The file \"{fileItem.FileName}\" cannot be edited, since it's not available on the filesystem.\n\n"+
                            "Please download it first.", "Error - File not downloaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    //Console.WriteLine(fileItem.FileInfo.FullName);
                    using (var form = new LoxStatFileForm(fileItem.FileInfo.FullName))
                    {
                        // Calculate the new location
                        int offsetX = 42; // Horizontal offset from the parent form
                        int offsetY = 70; // Vertical offset from the parent form
                        form.StartPosition = FormStartPosition.Manual; // Allows manual positioning
                        form.Location = new System.Drawing.Point(this.Location.X + offsetX, this.Location.Y + offsetY);

                        // Show the form as a dialog
                        form.ShowDialog(this);
                    }
                    RefreshLocal();
                    RefreshGridView();
                    break;
                case 8: //Upload
                    // show error message, if there is no file to upload
                    if (fileItem.FileInfo == null)
                    {
                        MessageBox.Show($"The file \"{fileItem.FileName}\" cannot be uploaded, since it's not available on the filesystem.\n\n"+
                            "Please download it first.", "Error - File not downloaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    progressBar.Maximum = 1;
                    progressBar.Value = 0;
                    progressBar.SetState(1);
                    progressLabel.Text = "Starting upload...";
                    if (Upload(fileItem))
                    {
                        progressBar.Value = 1;
                        progressLabel.Text = "Upload complete!";
                        RefreshMs();
                        RefreshGridView();
                    }
                    else
                    {
                        progressBar.Value = 1;
                        progressBar.SetState(2);
                        progressLabel.Text = "Upload failed!";
                    }
                    break;
                case 9: //Convert
                    // Ask user
                    if (fileItem.FileInfo != null)
                    {
                        // ToDo: ask for UUID of new meter
                        DialogResult result = MessageBox.Show($"Do you want to convert the file \"{fileItem.FileName}\" from the old meter format to the new one?\n"+
                            "NOTE: Under constructtion! Currently without any function.", "Question - Convert Stats File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            progressBar.Maximum = 1;
                            progressBar.Value = 0;
                            progressBar.SetState(1);
                            progressLabel.Text = "Starting converting...";
                            if (Convert(fileItem))
                            {
                                progressBar.Value = 1;
                                progressLabel.Text = "Converting complete!";
                                RefreshMs();
                                RefreshGridView();
                            }
                            else
                            {
                                progressBar.Value = 1;
                                progressBar.SetState(2);
                                progressLabel.Text = "Converting failed!";
                            }
                        }
                    }
                    break;
                case 10: //Delete
                    // show error message, if there is no file to upload
                    if ((fileItem.FileInfo != null) || (fileItem.MsFileInfo != null))
                    {
                        DialogResult result = MessageBox.Show($"Do you want to delete the file \"{fileItem.FileName}\" from the Loxone MS and local file system?", "Question - Delete Stats File from", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            progressBar.Maximum = 1;
                            progressBar.Value = 0;
                            progressBar.SetState(1);
                            progressLabel.Text = "Starting delete...";
                            if (Delete(fileItem))
                            {
                                progressBar.Value = 1;
                                progressLabel.Text = "Deleting complete!";
                                RefreshMs();
                                RefreshLocal();
                                RefreshGridView();
                            }
                            else
                            {
                                progressBar.Value = 1;
                                progressBar.SetState(2);
                                progressLabel.Text = "Deleting failed!";
                            }
                        }
                    }
                    break;
            }
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            progressBar.Maximum = _dataGridView.SelectedRows.Count;
            progressBar.Value = 0;
            progressBar.SetState(1);
            progressLabel.Text = "Starting download...";

            foreach(DataGridViewRow row in _dataGridView.SelectedRows)
            {
                int rowIndex = row.Index; // Capture the index for the closure
                bool result = await Task.Run(() => Download(_fileItems[rowIndex]));
                if (!result)
                {
                    progressBar.Value = progressBar.Maximum;
                    progressBar.SetState(2);
                    progressLabel.Text = "Download failed!";
                    return;
                }

                progressBar.Value++;
                progressLabel.Text = $"Downloaded {progressBar.Value} of {progressBar.Maximum}";
            }

            progressLabel.Text = "Download complete!";

            RefreshLocal();
            RefreshGridView();
        }

        private async void UploadButton_Click(object sender, EventArgs e)
        {
            progressBar.Maximum = _dataGridView.SelectedRows.Count;
            progressBar.Value = 0;
            progressBar.SetState(1);
            progressLabel.Text = "Starting upload...";

            foreach(DataGridViewRow row in _dataGridView.SelectedRows)
            {
                int rowIndex = row.Index; // Capture the index for the closure

                // show error message, if there is no file to upload
                if (_fileItems[rowIndex].FileInfo == null)
                {
                    progressBar.Value = progressBar.Maximum;
                    progressLabel.Text = "Upload failed!";
                    MessageBox.Show($"The file \"{_fileItems[rowIndex].FileName}\" cannot be uploaded, since it's not available on the filesystem.\n\nPlease download it first.", "Error - File not downloaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool result = await Task.Run(() => Upload(_fileItems[rowIndex]));
                if (!result)
                {
                    progressBar.Value = progressBar.Maximum;
                    progressBar.SetState(2);
                    progressLabel.Text = "Download failed!";
                    return;
                }

                progressBar.Value++;
                progressLabel.Text = $"Uploaded {progressBar.Value} of {progressBar.Maximum}";
            }
            progressLabel.Text = "Upload complete!";
        }

        // Launch project link
        private void githubLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/mr-manuel/Loxone_LoxStatEdit");
        }

        private void donateLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.md0.eu/links/loxstatsedit-donate");
        }

        #endregion

        private void aboutLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
