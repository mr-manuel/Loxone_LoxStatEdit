using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LoxStatEdit
{
    public class MsFileInfo
    {
        public string FileName { get; private set; }
        public DateTime Date { get; private set; }
        public int Size { get; private set; }

        public static IList<MsFileInfo> Load(Uri uri)
        {
            try
            {
                var list = new List<MsFileInfo>();
                var ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);

                // Log FTP output
                // File.AppendAllText("./custom.log", "\n\n- - - - -\n\n");

                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                using (var response = ftpWebRequest.GetResponse())
                using (var ftpStream = response.GetResponseStream())
                using (var streamReader = new StreamReader(ftpStream))
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    // Log FTP output
                    // File.AppendAllText("./custom.log", $"{line}\n");

                    // string pattern that matches Miniserver Gen 1 and Miniserver Gen 2
                    string pattern = @"[-rwx]{10}\s+[0-9]+\s+[0-9]+\s+[0-9]+\s+([0-9]+)\s+([A-Za-z]{3}\s+[0-9]{1,2}\s+[0-9:]+)\s+([0-9a-z_\-]+.)([0-9]{4})([0-9]{2})";
                    var result = Regex.Match(line, pattern);

                    if (result.Success)
                    {
                        var groups = result.Groups;
                        int.TryParse(groups[1].Value, out int size);

                        DateTime dateTime;
                        string dateString;
                        
                        if (groups[2].Value.Contains(":"))
                        {
                            dateString = groups[4].Value + " " + Regex.Replace(groups[2].Value, @"\s+", " ");
                        }
                        else
                        {
                            dateString = Regex.Replace(groups[2].Value, @"\s+", " ");
                        }
                        string[] formats = { "yyyy MMM dd HH:mm", "MMM dd yyyy", "yyyy MMM d HH:mm", "MMM d yyyy" };

                        if (!DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Handle the case where none of the formats match
                            MessageBox.Show($"The date \"{dateString}\" could not be matched with one of the following formats:\n{string.Join("\n", formats)}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return null;
                        }

                        var fileName = groups[3].Value + groups[4].Value + groups[5].Value;
                        
                        list.Add(new MsFileInfo
                        {
                            FileName = fileName,
                            Date = dateTime,
                            Size = size,
                        });

                        // Log FTP output
                        // File.AppendAllText("./custom.log", $"|- Filename: {fileName} - Date: {dateTime} - Size: {size}\n\n");

                    }
                }
                return list;
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                {
                    MessageBox.Show(ex.Message, "Error  - FTP connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\n\nData: {ex.Data}\n\nStackTrace: {ex.StackTrace}", "Error - IList", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }
    }
}
