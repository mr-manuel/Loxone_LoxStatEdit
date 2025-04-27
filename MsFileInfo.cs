using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LoxStatEdit
{
    public class MsFileInfo
    {
        public string FileName { get; private set; }
        public DateTime Date { get; private set; }
        public int Size { get; private set; }
        public bool Valid { get; private set; }

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
                    string pattern = @"[-rwx]{10}\s+[0-9]+\s+[0-9]+\s+[0-9]+\s+([0-9]+)\s+([A-Za-z]{3}\s+[0-9]{1,2}\s+[0-9:]+)\s+([0-9A-Za-z_\-\.]+)";
                    // Regex to parse entries from FTP LIST command - Loxone miniserver Gen.1 is using the following format for files in /stats directory
                    // examples:
                    // -rw-rw-rw-  1 0 0 691248 Nov 08 21:01 0c542aec-0252-e271-ffff9d266de2d576.201809
                    // -rw-rw-rw-  1 0 0  66896 Feb 29 23:00 1482ee72-032f-a20a-ffffa5f1a53e2e44.202402
                    // -rw-rw-rw-  1 0 0   1168 May 01 00:00 1ccec2f9-011e-ac27-ffffefc088fafadd_1.202404
                    // 1st group (): file size 
                    // 2nd group (): date and time in one of the following formats: MMM dd HH:mm, MMM d HH:mm, MMM dd yyyy, MMM d yyyy
                    // 3th group (): filename
                    var result = Regex.Match(line, pattern);

                    if (result.Success)
                    {
                        var groups = result.Groups;
                        int.TryParse(groups[1].Value, out int size);

                        var isFeb29 = false;
                        DateTime dateTime;
                        string dateString = Regex.Replace(groups[2].Value, @"\s+", " ");
                        string[] formats = { "MMM dd HH:mm", "MMM dd yyyy", "MMM d HH:mm", "MMM d yyyy" };

                        // special handling for Feb 29 to avoid invalid parsing without a year: add a day that will be subtracted later
                        if (dateString.StartsWith("Feb 29"))
                        {
                            dateString = $"Mar 01 {groups[2].Value.Substring(7)}";
                            dateString = Regex.Replace(dateString, @"\s+", " ");
                            isFeb29 = true;
                        }

                        if (!DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Handle the case where none of the formats matches
                            MessageBox.Show($"The date \"{dateString}\" could not be matched with one of the following formats:\n{string.Join("\n", formats)}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                        // time is in UTC, but we need local time
                        dateTime += TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);

                        if(isFeb29)
                        { 
                            // Removing the day that was added before
                            dateTime = dateTime.AddDays(-1);
                        }
                        if (dateTime >= DateTime.Now)
                        {
                            //filedate newer than now is not possible ... date must be from the last year
                            dateTime = dateTime.AddYears(-1);
                        }

                        var fileName = groups[3].Value;

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
