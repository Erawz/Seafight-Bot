using BoxyBot.Licensing.Security;
using BoxyBot.Util;
using System;
using System.Windows.Forms;

namespace BoxyBot
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            fingerprint = FingerPrint.Value();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            //loginForm _loginForm = new loginForm();
            //if (_loginForm.ShowDialog() == DialogResult.OK)
            //{
                Application.Run(new Form1());
            //}
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HelpTools.WriteToErrorLog(DateTime.Now.ToLongTimeString() + "\n\n<Unhandled Excpetion>\n\nObject: " + e.ExceptionObject + "\n\nIsTerminating: " + e.IsTerminating + "\n\nLog: " + Form1.form1.GetLines());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var data = "";
            foreach (var v in e.Exception.Data)
            {
                data += v.ToString() + "\n";
            }
            HelpTools.WriteToErrorLog(DateTime.Now.ToLongTimeString() + "\n\n<Thread Excpetion>\n\nException: " + e.Exception.ToString() + "\n\nSource: " + e.Exception.Source + "\n\nInner: " + e.Exception.InnerException + "\n\nData: " + data + "\n\nLog: " + Form1.form1.GetLines());
        }

        public const string compileTime = "17-07-12_13-31-09";
        public const string BountyBotVersion = "1.0 By Max !";
        public static string fingerprint = "";
    }
}
