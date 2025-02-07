# Changelog


## v1.0.6.2
* Added: Button to convert a file (not ready yet)
* Added: Button to delete file on MS and local file system
* Added: Columns for file size and file date
* Added: Red background and tooltip, if file is not using UUID plus .yyyyMM as filename
* Added: Tooltips in header row
* Changed: Fixed calcuation for modification date when file list is retrieved from MS via FTP handle modification date of Feb 29th (leap years) correctlytranslate modification time from UTC to local time
* Changed: Stability improvements


## v1.0.5.1

* Added: Show with cursor change that the `Index` column cannot be changed
* Changed: Code optimizations
* Changed: Fixed the problem when inserting the timestamp with `CTRL + V`
* Changed: Fixed a bug introduced with `v1.0.5.0` that cell formatting (colors/styles) where not shown anymore

## v1.0.5.0

* Added: Automatically add missing datapoints. This allows you for example to change the first or last date and easily fill up datapoints until that dates
* Added: Calculate datapoints: It's possible to select multiple rows or cells to calculate only that cells and also to select a row or cell and let then calculate all values below automatically
* Added: Filter statistic entries
* Added: Insert and delete datapoints
* Added: Select related datapoint when clicking on graph
* Changed: Optimized window opening positions
* Changed: Save sorting and selection arrow when refreshing file list
* Changed: Show a busy window, to let the user know something is happening when longer calculations are done

## v1.0.4.0

* Added: Allow user to sort by selected column
* Added: Browse button to browse local folders
* Added: Description column
* Added: Donation link
* Added: File Editor: Automatically load file when selected with the file browser
* Added: File Editor: Open current file folder when pressing browse
* Added: Many tooltip descriptions
* Added: More error handling and messages
* Added: Open button to open local folder in Windows Explorer
* Added: Progress bar for download/upload actions
* Added: Refresh automatically local file list after closing the File Editor
* Added: Some styling for better UX: Gray = Only on MS; Beige = Only on FS; Green = Newer/Larger on MS; Blue = Newer/Larger on FS
* Added: Upload selected button
* Changed: Name column now shows always the filename
* Changed: Small design fixes to increase responsiveness
* Changed: Sort columns by filename and not by name

## v1.0.3.0

* Changed: Fixed Win32-FileTime error

## v1.0.2.0

* Added: Support Miniserver newer generation for FTP download/upload by @mr-manuel

## v1.0.1.1

* Added: FTP error handling for upload and download by @mr-manuel
* Added: Message that Miniserver newer generations are not supported by @mr-manuel
* Added: Notes on how to apply modified statistics (in save dialog) by @mr-manuel
* Changed: Edit | Download | Upload to Download | Edit | Upload by @mr-manuel

## v1.0.1.0

* Added: FTP error handling for fetching filelist by @mr-manuel
* Added: Keyboard shortcuts by @mr-manuel

  **Miniserver Browser**
  * `ALT + M`: Refresh MS
  * `ALT + F`: Refresh FS
  * Press `Enter` in the `Miniserver` input field: Refresh MS
  * Press `Enter` in the `Working Folder` input field: Refresh FS

  **File Editor**
  * `ALT + B`: Browse
  * `ALT + L`: Load
  * `ALT + P`: Problems
  * `ALT + S`: Save

* Added: Version info and GitHub link by @mr-manuel
* Changed: Fixed button position in statistic editor by @mr-manuel
* Changed: Fixed error when # was in password by @mr-manuel
