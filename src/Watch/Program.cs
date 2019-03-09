using System;
using PowerArgs;

namespace Watch
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleArgs parsed = null;
            try
            {
                Console.WriteLine();
                Args.InvokeAction<ConsoleArgs>(args);
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<ConsoleArgs>());
            }

            // exit if help is requested
            if (parsed == null || parsed.Help)
            {
                return;
            }
        }
    }
}
