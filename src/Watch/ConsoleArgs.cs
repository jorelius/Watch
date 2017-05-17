using PowerArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Watch
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class ConsoleArgs
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Watch system clipboard for changes"), ArgShortcut("c")]
        public void Clipboard(ClipboardArgs args)
        {
            bool stopThread = false;
            var t = new Thread(new ThreadStart(() =>
            {
                string previousClipText = System.Windows.Clipboard.GetText();
                while (!stopThread)
                {
                    string currentText = String.Empty;
                    if (System.Windows.Clipboard.ContainsText() &&
                        !String.Equals((currentText = System.Windows.Clipboard.GetText()), previousClipText))
                    {
                        // save the text so we can compare it
                        // later
                        previousClipText = currentText;

                        // run program
                        Run(args.TargetProgram, args.Arguments.Replace("{CLIPTEXT}", currentText));
                    }

                    Thread.Sleep(args.Interval);
                }
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            Console.WriteLine("Press Key to End");
            Console.ReadKey();
            stopThread = true;
            t.Abort();
        }        
        
        [ArgActionMethod, ArgDescription("Watch file or directory for changes"), ArgShortcut("fs")]
        public void Filesystem(FileSystemArgs args)
        {
            bool stopThread = false;
            var t = new Thread(new ParameterizedThreadStart((stop) =>
            {
                var isFile = File.Exists(args.Path);
                var watcher = new FileSystemWatcher(Path.GetDirectoryName(args.Path), 
                    isFile ? Path.GetFileName(args.Path) : "");
                watcher.EnableRaisingEvents = true;

                while (!stopThread)
                {
                    watcher.WaitForChanged(WatcherChangeTypes.All);

                    // run program
                    Run(args.TargetProgram, args.Arguments);
                }
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            Console.WriteLine("Press Key to End");
            Console.ReadKey();
            stopThread = true;
            t.Abort();
        }

        [ArgActionMethod, ArgDescription("Watch for changes in http response")]
        public void Http(HttpArgs args)
        {
            throw new NotImplementedException("Http watching is not implemented yet");
        }

        [ArgActionMethod, ArgDescription("Watch for changes in Ftp response")]
        public void FTP(FtpArgs args)
        {
            bool stopThread = false;
            var t = new Thread(new ThreadStart(() =>
            {
                DateTime defaultDateTime = DateTime.Now.AddYears(-10000);
                DateTime previouslastModified = defaultDateTime; 
                while (!stopThread)
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(args.Uri);
                    request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    request.Credentials = new NetworkCredential(args.UserName, args.Password);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        // set initial modified state
                        if (previouslastModified == defaultDateTime)
                        {
                            previouslastModified = response.LastModified; 
                        }

                        // check datetime
                        if(previouslastModified < response.LastModified)
                        {
                            previouslastModified = response.LastModified;

                            // run program
                            Run(args.TargetProgram, args.Arguments);
                        }
                    }
                    
                    Thread.Sleep(args.Interval);
                }
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            Console.WriteLine("Press Key to End");
            Console.ReadKey();
            stopThread = true;
            t.Abort();
        }
        
        private void Run(string TargetProgram, string Arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = TargetProgram;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Arguments;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }
    }

    public class DefaultTriggerArgs
    {
        [ArgRequired, ArgDescription("The target program that is triggered"), ArgShortcut("t"), ArgPosition(1), ArgExistingFile]
        public string TargetProgram { get; set; }

        [ArgDescription("Arguments passed to program when Clipboard is updated. A {CLIPTEXT} token can be include in program arguments that will be replaced with clipboard text results upon trigger."), ArgShortcut("a"), ArgPosition(2)]
        public string Arguments { get; set; }
    }

    public class ClipboardArgs : DefaultTriggerArgs
    {
        [ArgDescription("Interval in which to poll clipboard in ms"), ArgShortcut("i"), DefaultValue(500), ArgRange(0, int.MaxValue)]
        public int Interval { get; set; }
    }

    public class FileSystemArgs : DefaultTriggerArgs
    {
        [ArgRequired, ArgDescription("Path to file or directory to be monitored"), ArgShortcut("p"), ArgPosition(3)]
        public string Path { get; set; }
    }

    public class HttpArgs : DefaultTriggerArgs
    {
        [ArgDescription("Interval in which to poll http resource in ms"), ArgShortcut("i"), DefaultValue(5000), ArgRange(0, int.MaxValue)]
        public int Interval { get; set; }

        [ArgRequired, ArgDescription("Uri to resource"), ArgShortcut("u"), ArgPosition(3)]
        public Uri Uri { get; set; }
    }
    
    public class FtpArgs : DefaultTriggerArgs
    {
        [ArgRequired, ArgDescription("account username"), ArgShortcut("un"), ArgPosition(3)]
        public string UserName { get; set; }

        [ArgRequired, ArgDescription("account password"), ArgShortcut("pw"), ArgPosition(4)]
        public string Password { get; set; }

        [ArgDescription("Interval in which to poll ftp resource in ms"), ArgShortcut("i"), DefaultValue(5000), ArgRange(0, int.MaxValue)]
        public int Interval { get; set; }

        [ArgRequired, ArgDescription("Uri to resource"), ArgShortcut("u"), ArgPosition(5)]
        public Uri Uri { get; set; }
    }
}
