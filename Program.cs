using System;
using System.Windows.Forms;

namespace mission
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Check if we should run in test mode
            bool runTests = args.Length > 0 && args[0] == "--test";

            if (runTests)
            {
                // Run API tests
                Console.WriteLine("Starting API tests...");
                var apiTests = new RamApi.ApiTests.ApiIntegrationTests();
                apiTests.RunTests();
                Console.WriteLine("API tests finished. Press any key to exit.");
                Console.ReadKey();
            }
            else
            {
                try
                {
                    // Run as Windows Forms application
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.Run(new MainForm());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting application: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}