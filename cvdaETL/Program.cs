using System.Runtime.InteropServices;
using CommandLine;
using cvdaETL;
using cvdaETL.Data;
using cvdaETL.Services;
using cvdaETL.Services.CsvHelper;
using cvdaETL.Services.ETLManager;
using cvdaETL.Utilities;

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Allows you to use the configuration in your application
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

static string GetConnectionString()
{
    var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddIniFile("config.ini", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // ...
    })
    .Build();

    // Now you can access the configuration throughout your application
    var configuration = builder.Services.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    builder.Run();
    
    return connectionString;
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Adding Logging to the console application using Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cvdalog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
/////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Start the application
Console.WriteLine("Hello, DarkStar!");

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunWithOptions)
    .WithNotParsed(HandleParseError);
//////////////////////////////////////////////////////////////////////////////////////////////////////////

static void RunWithOptions(Options opts)
{
    var connectionString = GetConnectionString();
    Console.WriteLine($"Database located.... ");
    Repo.Initialize(connectionString, opts.CsvPath); // repo is a static singleton

    string[] csvFiles = Directory.GetFiles(opts.CsvPath, "*.csv");
    foreach (string csvFile in csvFiles) CSVUtilities.Filter(csvFile);

    //Work on updating each table in turn from outside in. Guids as primary keys
    // Looking to update or insert rather than delete and insert
    // In this order
    // 1. Patients
    // 2. Conditions
    // 3. Targets
    // 4. RecallTeam
    // 5. Staff
    // 6. Clinics
    // 7. Interactions
    // 8. Appointments
    // 9. Observations
    //
    // Registers are a bit different as they are a bit more complex
    // Update Registers as you go along the ETL process
    // One class per import passing the Repo object

    Console.WriteLine($"Importing Patients...");
    new PatientProcessor().ImportPatients();

    Console.WriteLine("Goodbye🦾...");
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
}

static void HandleParseError(IEnumerable<Error> errs)
{
    // Handle errors in command-line argument parsing
    foreach (var err in errs)
    {
        Console.WriteLine($"Error: {err.Tag}");
    }
}