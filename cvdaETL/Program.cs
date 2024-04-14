using System.Runtime.InteropServices;
using CommandLine;
using cvdaETL;
using cvdaETL.Data;
using cvdaETL.Services;
using cvdaETL.Services.CsvHelper;

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Allows you to use the configuration in your application
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
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
    var outputFolder = Path.Combine(opts.FolderPath, "Output");
    if (Directory.Exists(outputFolder)) Directory.Delete(outputFolder,true);

    Console.WriteLine($"Creating Repository.... {opts.FolderPath}");
    var repo = new Repo(opts.FolderPath);

    Directory.CreateDirectory(outputFolder);
    

    Console.WriteLine($"Importing in csvs...");
    foreach (var csvFile in repo.Files)
    {
        var filename = Path.Combine(opts.FolderPath, csvFile);
        var records = new CsvHelperManager().ReadCsvFile(filename);
        repo.CvdaModels.AddRange(records);
    }

    Console.WriteLine($"Exporting csvs to {outputFolder}..");

    new CsvHelperManager().WriteCsvFile(repo.CvdaModels, Path.Combine(outputFolder, "CVDATargets.csv"));

    Console.WriteLine("Enjoy...");
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