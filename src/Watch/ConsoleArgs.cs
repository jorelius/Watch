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
            var t = new Thread(new ParameterizedThreadStart((stop) =>
            {
                string previousClipText = System.Windows.Clipboard.GetText();
                while (!(bool)stop)
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

                    Thread.Sleep(500);
                }
            }));

            t.SetApartmentState(ApartmentState.STA);

            bool stopThread = false;
            t.Start(stopThread);

            Console.WriteLine("Press Key to End");
            Console.ReadKey();
            stopThread = true;
            t.Abort();
        }        
        
        [ArgActionMethod, ArgDescription("Watch file or directory for changes"), ArgShortcut("fs")]
        public void Filesystem(FileSystemArgs args)
        {
            var t = new Thread(new ParameterizedThreadStart((stop) =>
            {
                var isFile = File.Exists(args.Path);
                var watcher = new FileSystemWatcher(Path.GetDirectoryName(args.Path), 
                    isFile ? Path.GetFileName(args.Path) : "");
                watcher.EnableRaisingEvents = true;

                while (!(bool)stop)
                {
                    watcher.WaitForChanged(WatcherChangeTypes.All);

                    // run program
                    Run(args.TargetProgram, args.Arguments);
                }
            }));

            t.SetApartmentState(ApartmentState.STA);

            bool stopThread = false;
            t.Start(stopThread);

            Console.WriteLine("Press Key to End");
            Console.ReadKey();
            stopThread = true;
            t.Abort();
        }

        [ArgActionMethod, ArgDescription("Watch for changes in http response")]
        public void Http()
        {
            throw new NotImplementedException("Http watching is not implemented yet");
        }

        [ArgActionMethod, ArgDescription("Watch for changes in Ftp response")]
        public void FTP()
        {
            throw new NotImplementedException("FTP watching is not implemented yet");
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
    
    public class ClipboardArgs
    {
        [ArgRequired, ArgDescription("The target program that is triggered"), ArgShortcut("t"), ArgPosition(1), ArgExistingFile]
        public string TargetProgram { get; set; }

        [ArgDescription("Arguments passed to program when Clipboard is updated. A {CLIPTEXT} token can be include in program arguments that will be replaced with clipboard text results upon trigger."), ArgShortcut("a"), ArgPosition(2)]
        public string Arguments { get; set; }
    }

    public class FileSystemArgs
    {
        [ArgRequired, ArgDescription("The target program that is triggered"), ArgShortcut("t"), ArgPosition(1), ArgExistingFile]
        public string TargetProgram { get; set; }

        [ArgDescription("Arguments passed to program when Clipboard is updated"), ArgShortcut("a"), ArgPosition(2)]
        public string Arguments { get; set; }

        [ArgRequired, ArgDescription("Path to file or directory to be monitored"), ArgShortcut("p"), ArgPosition(3)]
        public string Path { get; set; }
    }
}
