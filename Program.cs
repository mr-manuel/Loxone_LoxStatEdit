using LoxStatEdit.Properties;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LoxStatEdit
{
    public static class Program
    {
        /* TODOs:
         * - XML-Import/Export
         * - FTP-Upload/Download
         */

        internal static void HandleException(object exception)
        {
            var owner = Application.OpenForms.Cast<IWin32Window>().FirstOrDefault();
            var control = owner as Control;
            MessageBox.Show(owner, exception.ToString(),
                control == null ? "Error" : control.Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        /// <summary>The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += Application_ThreadException;
                Application.Run(new MiniserverForm(args));
            }
            catch(Exception ex)
            {
                HandleException(ex);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }
    }
}
