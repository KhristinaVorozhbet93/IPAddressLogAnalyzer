using IPAddressLogAnalyzer;
using IPAddressLogAnalyzer.Interfaces;
using IPAddressLogAnalyzer.Services;
using Microsoft.Extensions.Configuration;
using System.Globalization;

class Program
{
    static async Task Main(string[] args)
    {
        //if (args.Length == 0)
        //{
        //    Console.WriteLine("Задайте параметры ");
        //    return;
        //}
        ////Переменные среды окружения
        //var ipConfiguration = GetIPConfigurationFromEnvironmentVariable();

        //Командная строка
        //var ipConfiguration = GetIPConfigurationFromCommandPrompt(args);

        //Файл конфигурации
        var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        var ipConfigSection = configuration
            .GetRequiredSection("IPConfiguration");
        if (ipConfigSection is null)
        {
            throw new InvalidOperationException("IpConfig не заданы");
        }
        var ipConfiguration = GetIPConfigurationFromConfigurationFile(ipConfigSection);

        IIPAddressFileService ipFilesService = new IPAddressFileService();
        IPService ipService = new(ipFilesService, ipConfiguration);
        await ipService.WriteIPAddressesWithConfigurationsToFile();
    }

    static IPConfiguration GetIPConfigurationFromCommandPrompt(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var fileLog = GetParameterValue(args, "--file-log");
        if (string.IsNullOrEmpty(fileLog))
        {
            throw new ArgumentException(nameof(fileLog));
        }
        var fileOutput = GetParameterValue(args, "--file-output");

        if (string.IsNullOrEmpty(fileOutput))
        {
            throw new ArgumentException(nameof(fileOutput));
        }
        var timeStartString = GetParameterTimeValue(args, "--time-start");
        if (string.IsNullOrEmpty(timeStartString))
        {
            throw new ArgumentException(nameof(timeStartString));
        }
        var timeEndString = GetParameterTimeValue(args, "--time-end");
        if (string.IsNullOrEmpty(timeEndString))
        {
            throw new ArgumentException(nameof(timeEndString));
        }
        var addressStart = GetParameterValue(args, "--address-start");
        var addressMask = GetParameterValue(args, "--address-mask");

        return ParseIPConfigurationData(fileLog, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
    }
    static IPConfiguration GetIPConfigurationFromConfigurationFile(IConfigurationSection ipConfigSection)
    {
        ArgumentNullException.ThrowIfNull(ipConfigSection);
        var fileLog = ipConfigSection["file_log"];
        if (string.IsNullOrEmpty(fileLog))
        {
            throw new ArgumentException(nameof(fileLog));
        }
        var fileOutput = ipConfigSection["file_output"];
        if (string.IsNullOrEmpty(fileOutput))
        {
            throw new ArgumentException(nameof(fileOutput));
        }
        var timeStartString = ipConfigSection["time_start"];
        if (string.IsNullOrEmpty(timeStartString))
        {
            throw new ArgumentException(nameof(timeStartString));
        }
        var timeEndString = ipConfigSection["time_end"];
        if (string.IsNullOrEmpty(timeEndString))
        {
            throw new ArgumentException(nameof(timeEndString));
        }
        var addressStart = ipConfigSection["address_start"];
        var addressMask = ipConfigSection["address_mask"];

        return ParseIPConfigurationData(fileLog, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
    }
    static IPConfiguration GetIPConfigurationFromEnvironmentVariable()
    {
        var fileLog = Environment.GetEnvironmentVariable("--file-log");
        if (string.IsNullOrEmpty(fileLog))
        {
            throw new ArgumentException(nameof(fileLog));
        }
        var fileOutput = Environment.GetEnvironmentVariable("--file-output");
        if (string.IsNullOrEmpty(fileOutput))
        {
            throw new ArgumentException(nameof(fileOutput));
        }
        var timeStartString = Environment.GetEnvironmentVariable("--time-start");
        if (string.IsNullOrEmpty(timeStartString))
        {
            throw new ArgumentException(nameof(timeStartString));
        }
        var timeEndString = Environment.GetEnvironmentVariable("--time-end");
                if (string.IsNullOrEmpty(timeEndString))
        {
            throw new ArgumentException(nameof(timeEndString));
        }
        var addressStart = Environment.GetEnvironmentVariable("--address-start");
        var addressMask = Environment.GetEnvironmentVariable("--address-mask");

        return ParseIPConfigurationData(fileLog!, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
    }
    private static IPConfiguration ParseIPConfigurationData
        (string fileLog, string fileOutput, string timeStartString, string timeEndString,
        string? addressStart, string? addressMask)
    {
        DateTime timeStart, timeEnd;
        CultureInfo provider = new CultureInfo("ru-RU");
        if (!DateTime.TryParseExact(timeStartString, "yyyy-MM-dd HH:mm:ss", provider, DateTimeStyles.None, out timeStart))
            throw new FormatException("Не удалось преобразовать тип данных string в DateTime");
        if (!DateTime.TryParseExact(timeEndString, "yyyy-MM-dd HH:mm:ss", provider, DateTimeStyles.None, out timeEnd))
            throw new FormatException("Не удалось преобразовать тип данных string в DateTime" );

        IPConfiguration configuration = new()
        {
            FileLog = fileLog,
            FileOutput = fileOutput,
            TimeStart = timeStart,
            TimeEnd = timeEnd,
            AddressMask = addressMask,
            AddressStart = addressStart
        };
        return configuration;
    }
    private static string? GetParameterValue(string[] args, string parameter)
    {
        int index = Array.IndexOf(args, parameter);
        return args[index + 1];
    }
    private static string? GetParameterTimeValue(string[] args, string parameter)
    {
        int index = Array.IndexOf(args, parameter);
        var time = $"{args[index + 1]} {args[index + 2]}";
        return time;
    }
}