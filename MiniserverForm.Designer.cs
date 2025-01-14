using System.Drawing;
using System.Windows.Forms;

namespace LoxStatEdit
{
    partial class MiniserverForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label urlLabel;
            System.Windows.Forms.Button refreshFolderButton;
            System.Windows.Forms.Button refreshMsButton;
            System.Windows.Forms.Label folderLabel;
            System.Windows.Forms.Button downloadButton;
            System.Windows.Forms.Button uploadButton;
            System.Windows.Forms.Label aboutLabel;
            System.Windows.Forms.Button openFolderButton;
            System.Windows.Forms.Button browseFolderButton;
            System.Windows.Forms.Label filterLabel;
            System.Windows.Forms.Button filterButton;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MiniserverForm));
            this._urlTextBox = new System.Windows.Forms.TextBox();
            this._folderTextBox = new System.Windows.Forms.TextBox();
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this._nameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._descriptionCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._yearMonthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._dateModifiedCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._sizeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._statusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._downloadCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this._editCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this._uploadCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this._convertCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this._deleteCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this.githubLabel = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._filterTextBox = new System.Windows.Forms.TextBox();
            this.donateLabel = new System.Windows.Forms.LinkLabel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            urlLabel = new System.Windows.Forms.Label();
            refreshFolderButton = new System.Windows.Forms.Button();
            refreshMsButton = new System.Windows.Forms.Button();
            folderLabel = new System.Windows.Forms.Label();
            downloadButton = new System.Windows.Forms.Button();
            uploadButton = new System.Windows.Forms.Button();
            aboutLabel = new System.Windows.Forms.Label();
            openFolderButton = new System.Windows.Forms.Button();
            browseFolderButton = new System.Windows.Forms.Button();
            filterLabel = new System.Windows.Forms.Label();
            filterButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // urlLabel
            // 
            urlLabel.Location = new System.Drawing.Point(24, 33);
            urlLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new System.Drawing.Size(140, 35);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "Miniserver:";
            // 
            // refreshFolderButton
            // 
            refreshFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            refreshFolderButton.Location = new System.Drawing.Point(1660, 73);
            refreshFolderButton.Margin = new System.Windows.Forms.Padding(6);
            refreshFolderButton.Name = "refreshFolderButton";
            refreshFolderButton.Size = new System.Drawing.Size(150, 44);
            refreshFolderButton.TabIndex = 6;
            refreshFolderButton.Text = "Refresh &FS";
            this.toolTip.SetToolTip(refreshFolderButton, "Refresh Computer files (ALT + F)");
            refreshFolderButton.UseVisualStyleBackColor = true;
            refreshFolderButton.Click += new System.EventHandler(this.RefreshFolderButton_Click);
            // 
            // refreshMsButton
            // 
            refreshMsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            refreshMsButton.Location = new System.Drawing.Point(1660, 23);
            refreshMsButton.Margin = new System.Windows.Forms.Padding(6);
            refreshMsButton.Name = "refreshMsButton";
            refreshMsButton.Size = new System.Drawing.Size(150, 44);
            refreshMsButton.TabIndex = 2;
            refreshMsButton.Text = "Refresh &MS";
            this.toolTip.SetToolTip(refreshMsButton, "Refresh Miniserver files (ALT + M)");
            refreshMsButton.UseVisualStyleBackColor = true;
            refreshMsButton.Click += new System.EventHandler(this.RefreshMsButton_Click);
            // 
            // folderLabel
            // 
            folderLabel.Location = new System.Drawing.Point(24, 81);
            folderLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            folderLabel.Name = "folderLabel";
            folderLabel.Size = new System.Drawing.Size(140, 35);
            folderLabel.TabIndex = 3;
            folderLabel.Text = "Computer:";
            // 
            // downloadButton
            // 
            downloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            downloadButton.Location = new System.Drawing.Point(1354, 1013);
            downloadButton.Margin = new System.Windows.Forms.Padding(6);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new System.Drawing.Size(222, 44);
            downloadButton.TabIndex = 9;
            downloadButton.Text = "&Download selected";
            this.toolTip.SetToolTip(downloadButton, "Download selected statistic files (ALT + D)");
            downloadButton.UseVisualStyleBackColor = true;
            downloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // uploadButton
            // 
            uploadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            uploadButton.Location = new System.Drawing.Point(1588, 1013);
            uploadButton.Margin = new System.Windows.Forms.Padding(6);
            uploadButton.Name = "uploadButton";
            uploadButton.Size = new System.Drawing.Size(222, 44);
            uploadButton.TabIndex = 10;
            uploadButton.Text = "&Upload selected";
            this.toolTip.SetToolTip(uploadButton, "Upload selected statistic files (ALT + U)");
            uploadButton.UseVisualStyleBackColor = true;
            uploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // aboutLabel
            // 
            aboutLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            aboutLabel.Location = new System.Drawing.Point(24, 1023);
            aboutLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            aboutLabel.Name = "aboutLabel";
            aboutLabel.Size = new System.Drawing.Size(342, 35);
            aboutLabel.TabIndex = 100;
            aboutLabel.Text = "LoxStatEdit v1.0.6.1 (2025.01.12)";
            aboutLabel.Click += new System.EventHandler(this.aboutLabel_Click);
            // 
            // openFolderButton
            // 
            openFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            openFolderButton.Location = new System.Drawing.Point(1498, 73);
            openFolderButton.Margin = new System.Windows.Forms.Padding(6);
            openFolderButton.Name = "openFolderButton";
            openFolderButton.Size = new System.Drawing.Size(150, 44);
            openFolderButton.TabIndex = 5;
            openFolderButton.Text = "&Open";
            this.toolTip.SetToolTip(openFolderButton, "Open folder in Windows Explorer (ALT + O)");
            openFolderButton.UseVisualStyleBackColor = true;
            openFolderButton.Click += new System.EventHandler(this.openFolderButton_Click);
            // 
            // browseFolderButton
            // 
            browseFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            browseFolderButton.Location = new System.Drawing.Point(1336, 73);
            browseFolderButton.Margin = new System.Windows.Forms.Padding(6);
            browseFolderButton.Name = "browseFolderButton";
            browseFolderButton.Size = new System.Drawing.Size(150, 44);
            browseFolderButton.TabIndex = 4;
            browseFolderButton.Text = "&Browse...";
            this.toolTip.SetToolTip(browseFolderButton, "Browse folders (ALT + B)");
            browseFolderButton.UseVisualStyleBackColor = true;
            browseFolderButton.Click += new System.EventHandler(this.BrowseFolderButton_Click);
            // 
            // filterLabel
            // 
            filterLabel.Location = new System.Drawing.Point(24, 131);
            filterLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            filterLabel.Name = "filterLabel";
            filterLabel.Size = new System.Drawing.Size(140, 35);
            filterLabel.TabIndex = 105;
            filterLabel.Text = "Filter entries:";
            // 
            // filterButton
            // 
            filterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            filterButton.Location = new System.Drawing.Point(1660, 123);
            filterButton.Margin = new System.Windows.Forms.Padding(6);
            filterButton.Name = "filterButton";
            filterButton.Size = new System.Drawing.Size(150, 44);
            filterButton.TabIndex = 8;
            filterButton.Text = "Filter";
            this.toolTip.SetToolTip(filterButton, "Filter the displayed entries");
            filterButton.UseVisualStyleBackColor = true;
            filterButton.Click += new System.EventHandler(this.FilterButton_Click);
            // 
            // _urlTextBox
            // 
            this._urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._urlTextBox.Location = new System.Drawing.Point(176, 27);
            this._urlTextBox.Margin = new System.Windows.Forms.Padding(6);
            this._urlTextBox.Name = "_urlTextBox";
            this._urlTextBox.Size = new System.Drawing.Size(1468, 31);
            this._urlTextBox.TabIndex = 1;
            this._urlTextBox.Text = "ftp://adminname:adminpassword@miniserver-ip-or-hostname:21";
            this.toolTip.SetToolTip(this._urlTextBox, "Miniserver connection details");
            this._urlTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._urlTextBox_KeyDown);
            // 
            // _folderTextBox
            // 
            this._folderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._folderTextBox.Location = new System.Drawing.Point(176, 77);
            this._folderTextBox.Margin = new System.Windows.Forms.Padding(6);
            this._folderTextBox.Name = "_folderTextBox";
            this._folderTextBox.Size = new System.Drawing.Size(1144, 31);
            this._folderTextBox.TabIndex = 3;
            this.toolTip.SetToolTip(this._folderTextBox, "Path to folder on this computer where statistic files are saved");
            this._folderTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._folderTextBox_KeyDown);
            // 
            // _dataGridView
            // 
            this._dataGridView.AllowUserToAddRows = false;
            this._dataGridView.AllowUserToDeleteRows = false;
            this._dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._nameCol,
            this._descriptionCol,
            this._yearMonthCol,
            this._dateModifiedCol,
            this._sizeCol,
            this._statusCol,
            this._downloadCol,
            this._editCol,
            this._uploadCol,
            this._convertCol,
            this._deleteCol});
            this._dataGridView.Location = new System.Drawing.Point(24, 179);
            this._dataGridView.Margin = new System.Windows.Forms.Padding(6);
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.RowHeadersWidth = 30;
            this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridView.Size = new System.Drawing.Size(1786, 821);
            this._dataGridView.TabIndex = 10;
            this._dataGridView.VirtualMode = true;
            this._dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentClick);
            this._dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.DataGridView_CellValueNeeded);
            // 
            // _nameCol
            // 
            this._nameCol.DataPropertyName = "Name";
            this._nameCol.HeaderText = "Name";
            this._nameCol.MinimumWidth = 50;
            this._nameCol.Name = "_nameCol";
            this._nameCol.ReadOnly = true;
            this._nameCol.ToolTipText = "Name of the file";
            this._nameCol.Width = 250;
            // 
            // _descriptionCol
            // 
            this._descriptionCol.DataPropertyName = "Description";
            this._descriptionCol.HeaderText = "Description";
            this._descriptionCol.MinimumWidth = 50;
            this._descriptionCol.Name = "_descriptionCol";
            this._descriptionCol.ReadOnly = true;
            this._descriptionCol.ToolTipText = "Defined in Loxone Config";
            this._descriptionCol.Width = 250;
            // 
            // _yearMonthCol
            // 
            this._yearMonthCol.DataPropertyName = "YearMonth";
            dataGridViewCellStyle1.Format = "yyyy-MM";
            this._yearMonthCol.DefaultCellStyle = dataGridViewCellStyle1;
            this._yearMonthCol.HeaderText = "Year/Month";
            this._yearMonthCol.MinimumWidth = 50;
            this._yearMonthCol.Name = "_yearMonthCol";
            this._yearMonthCol.ReadOnly = true;
            this._yearMonthCol.ToolTipText = "From filename extension";
            this._yearMonthCol.Width = 200;
            // 
            // _dateModifiedCol
            // 
            this._dateModifiedCol.HeaderText = "Date modified";
            this._dateModifiedCol.MinimumWidth = 70;
            this._dateModifiedCol.Name = "_dateModifiedCol";
            this._dateModifiedCol.ReadOnly = true;
            this._dateModifiedCol.ToolTipText = "Date when the file was modified. NOTE: file that were downloaded via FTP have a d" +
    "ate with tiis or last year.";
            this._dateModifiedCol.Width = 140;
            // 
            // _sizeCol
            // 
            this._sizeCol.HeaderText = "Size";
            this._sizeCol.MinimumWidth = 60;
            this._sizeCol.Name = "_sizeCol";
            this._sizeCol.ReadOnly = true;
            this._sizeCol.ToolTipText = "Size of file on FS (local system)";
            this._sizeCol.Width = 200;
            // 
            // _statusCol
            // 
            this._statusCol.DataPropertyName = "StatusString";
            this._statusCol.HeaderText = "Status";
            this._statusCol.MinimumWidth = 50;
            this._statusCol.Name = "_statusCol";
            this._statusCol.ReadOnly = true;
            this._statusCol.ToolTipText = "Result of comparision between file on Loxone MS and FS (local system)";
            this._statusCol.Width = 200;
            // 
            // _downloadCol
            // 
            this._downloadCol.HeaderText = "Download";
            this._downloadCol.MinimumWidth = 8;
            this._downloadCol.Name = "_downloadCol";
            this._downloadCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._downloadCol.Text = "Download";
            this._downloadCol.ToolTipText = "Copy file from Loxone MS to FS (local system)";
            this._downloadCol.Width = 60;
            // 
            // _editCol
            // 
            this._editCol.HeaderText = "Edit";
            this._editCol.MinimumWidth = 20;
            this._editCol.Name = "_editCol";
            this._editCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._editCol.Text = "Edit";
            this._editCol.ToolTipText = "Edit entries of this file on FS (local system)";
            this._editCol.Width = 50;
            // 
            // _uploadCol
            // 
            this._uploadCol.HeaderText = "Upload";
            this._uploadCol.MinimumWidth = 20;
            this._uploadCol.Name = "_uploadCol";
            this._uploadCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._uploadCol.Text = "Upload";
            this._uploadCol.ToolTipText = "Copy file from FS (local system) to Loxone MS";
            this._uploadCol.Width = 60;
            // 
            // _convertCol
            // 
            this._convertCol.HeaderText = "Convert";
            this._convertCol.MinimumWidth = 20;
            this._convertCol.Name = "_convertCol";
            this._convertCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._convertCol.Text = "Convert";
            this._convertCol.ToolTipText = "Convert data from old meter function block to new style";
            this._convertCol.Width = 60;
            // 
            // _deleteCol
            // 
            this._deleteCol.HeaderText = "Delete";
            this._deleteCol.MinimumWidth = 20;
            this._deleteCol.Name = "_deleteCol";
            this._deleteCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._deleteCol.Text = "Delete";
            this._deleteCol.ToolTipText = "Delete file on Loxone MS or FS (local system) after confirmation";
            this._deleteCol.Width = 60;
            // 
            // githubLabel
            // 
            this.githubLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.githubLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 10);
            this.githubLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.githubLabel.LinkColor = System.Drawing.Color.Blue;
            this.githubLabel.Location = new System.Drawing.Point(368, 1025);
            this.githubLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.githubLabel.Name = "githubLabel";
            this.githubLabel.Size = new System.Drawing.Size(84, 35);
            this.githubLabel.TabIndex = 101;
            this.githubLabel.TabStop = true;
            this.githubLabel.Text = "GitHub";
            this.githubLabel.UseCompatibleTextRendering = true;
            this.githubLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.githubLinkLabel_LinkClicked);
            // 
            // _filterTextBox
            // 
            this._filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._filterTextBox.Location = new System.Drawing.Point(176, 127);
            this._filterTextBox.Margin = new System.Windows.Forms.Padding(6);
            this._filterTextBox.Name = "_filterTextBox";
            this._filterTextBox.Size = new System.Drawing.Size(1468, 31);
            this._filterTextBox.TabIndex = 7;
            this.toolTip.SetToolTip(this._filterTextBox, "Filter entries by file or description");
            this._filterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._filterTextBox_KeyDown);
            // 
            // donateLabel
            // 
            this.donateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.donateLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 20);
            this.donateLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.donateLabel.LinkColor = System.Drawing.Color.Blue;
            this.donateLabel.Location = new System.Drawing.Point(464, 1025);
            this.donateLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.donateLabel.Name = "donateLabel";
            this.donateLabel.Size = new System.Drawing.Size(192, 35);
            this.donateLabel.TabIndex = 102;
            this.donateLabel.TabStop = true;
            this.donateLabel.Text = "Donate && Support";
            this.donateLabel.UseCompatibleTextRendering = true;
            this.donateLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.donateLabel_LinkClicked);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(1022, 1013);
            this.progressBar.Margin = new System.Windows.Forms.Padding(6);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(320, 44);
            this.progressBar.TabIndex = 103;
            // 
            // progressLabel
            // 
            this.progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel.AutoSize = true;
            this.progressLabel.BackColor = System.Drawing.Color.Transparent;
            this.progressLabel.Location = new System.Drawing.Point(1032, 1023);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(46, 25);
            this.progressLabel.TabIndex = 104;
            this.progressLabel.Text = "Idle";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MiniserverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1834, 1079);
            this.Controls.Add(filterButton);
            this.Controls.Add(this._filterTextBox);
            this.Controls.Add(filterLabel);
            this.Controls.Add(refreshMsButton);
            this.Controls.Add(browseFolderButton);
            this.Controls.Add(openFolderButton);
            this.Controls.Add(refreshFolderButton);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(downloadButton);
            this.Controls.Add(uploadButton);
            this.Controls.Add(aboutLabel);
            this.Controls.Add(this.githubLabel);
            this.Controls.Add(this.donateLabel);
            this.Controls.Add(this._dataGridView);
            this.Controls.Add(urlLabel);
            this.Controls.Add(this._urlTextBox);
            this.Controls.Add(folderLabel);
            this.Controls.Add(this._folderTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MinimumSize = new System.Drawing.Size(1474, 454);
            this.Name = "MiniserverForm";
            this.Text = "Loxone Stats Editor - Miniserver Browser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MiniserverForm_Closing);
            this.Load += new System.EventHandler(this.MiniserverForm_Load);
            this.ResizeEnd += new System.EventHandler(this.MiniserverForm_ResizEnd);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _urlTextBox;
        private System.Windows.Forms.TextBox _folderTextBox;
        private System.Windows.Forms.DataGridView _dataGridView;
        private LinkLabel githubLabel;
        private ToolTip toolTip;
        private LinkLabel donateLabel;
        private ProgressBar progressBar;
        private Label progressLabel;
        private FolderBrowserDialog folderBrowserDialog;
        private TextBox _filterTextBox;
        private DataGridViewTextBoxColumn _nameCol;
        private DataGridViewTextBoxColumn _descriptionCol;
        private DataGridViewTextBoxColumn _yearMonthCol;
        private DataGridViewTextBoxColumn _dateModifiedCol;
        private DataGridViewTextBoxColumn _sizeCol;
        private DataGridViewTextBoxColumn _statusCol;
        private DataGridViewButtonColumn _downloadCol;
        private DataGridViewButtonColumn _editCol;
        private DataGridViewButtonColumn _uploadCol;
        private DataGridViewButtonColumn _convertCol;
        private DataGridViewButtonColumn _deleteCol;
    }
}