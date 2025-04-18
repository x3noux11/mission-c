using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
using RamApi.ApiTests; // Added for API testing
namespace mission
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            // ApplicationConfiguration.Initialize(); // Commented out for API testing
            // Application.Run(new MainForm()); // Commented out for API testing

            // Run API tests
            Console.WriteLine("Starting API tests...");
            var apiTests = new RamApi.ApiTests.ApiIntegrationTests();
            apiTests.RunTests();
            Console.WriteLine("API tests finished. Press any key to exit.");
            Console.ReadKey();
        }
    }
}