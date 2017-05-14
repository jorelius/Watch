using PowerArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var t = new Thread(new ParameterizedThreadStart((stop) =>
            {
                string previousClipText = String.Empty;
                while (!(bool)stop)
                {
                    string currentText = String.Empty;
                    if (System.Windows.Clipboard.ContainsText() &&
                        !String.Equals((currentText = System.Windows.Clipboard.GetText()), previousClipText))
                    {
                        // save the text so we can compare it
                        // later
                        previousClipText = currentText;

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.CreateNoWindow = false;
                        startInfo.UseShellExecute = false;
                        startInfo.FileName = args.TargetProgram;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.Arguments = args.Arguments.Replace("{CLIPTEXT}", currentText); // token replacement

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

                    Thread.Sleep(500);
            }
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start(stopThread);

            Console.WriteLine("Press Key to End");
            Console.ReadKey();
            stopThread = true;
            t.Abort();
        }        


        [ArgActionMethod, ArgDescription("Watch file or directory for changes"), ArgShortcut("f")]
        public void Filesystem()
        {
            throw new NotImplementedException("Filesystem watching is not implemented yet");
        }

        [ArgActionMethod, ArgDescription("Watch for changes in http response"), ArgShortcut("h")]
        public void Http()
        {
            throw new NotImplementedException("Http watching is not implemented yet");
        }

        public void FTP()
        {
            throw new NotImplementedException("FTP watching is not implemented yet");
        }
    }

    public class ClipboardArgs
    {
        [ArgRequired, ArgDescription("The target program that is triggered"), ArgShortcut("t"), ArgPosition(1), ArgExistingFile]
        public string TargetProgram { get; set; }

        [ArgDescription("Arguments passed program when Clipboard is updated"), ArgShortcut("a"), ArgPosition(2)]
        public string Arguments { get; set; }
    }
}
