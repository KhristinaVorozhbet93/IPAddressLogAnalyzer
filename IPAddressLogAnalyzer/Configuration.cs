using IPAddressLogAnalyzer;
using Microsoft.Extensions.Configuration;
using System.Globalization;

public class Configuration
{
    /// <summary>
    /// Метод, который получает параметры через конфигурационный файл 
    /// </summary>
    /// <param name="ipConfigSection">Секция с конфигурационного файла</param>
    /// <returns>Возвращает объект IPConfiguration</returns>
    /// <exception cref="ArgumentException">Исключение, если в метод для параметра передается некорректное значение</exception>
    public IPConfiguration GetIPConfigurationFromConfigurationFile(IConfigurationSection ipConfigSection)
    {
        ArgumentNullException.ThrowIfNull(ipConfigSection);
        var fileLog = ipConfigSection["file_log"];
        if (string.IsNullOrWhiteSpace(fileLog))
        {
            throw new ArgumentException(nameof(fileLog));
        }
        var fileOutput = ipConfigSection["file_output"];
        if (string.IsNullOrWhiteSpace(fileOutput))
        {
            throw new ArgumentException(nameof(fileOutput));
        }
        var timeStartString = ipConfigSection["time_start"];
        if (string.IsNullOrEmpty(timeStartString))
        {
            throw new ArgumentException(nameof(timeStartString));
        }
        var timeEndString = ipConfigSection["time_end"];
        if (string.IsNullOrWhiteSpace(timeEndString))
        {
            throw new ArgumentException(nameof(timeEndString));
        }
        var addressStart = ipConfigSection["address_start"];
        var addressMask = ipConfigSection["address_mask"];

        return ParseIPConfigurationData(fileLog, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
    }
    /// <summary>
    /// Метод, который получает параметры через переменные среды окружения
    /// </summary>
    /// <returns>Возвращает объект IPConfiguration</returns>
    /// <exception cref="ArgumentException">Исключение, если в метод для параметра передается некорректное значение</exception>
    public IPConfiguration GetIPConfigurationFromEnvironmentVariable()
    {
        var fileLog = Environment.GetEnvironmentVariable("--file-log");
        if (string.IsNullOrWhiteSpace(fileLog))
        {
            throw new ArgumentException(nameof(fileLog));
        }
        var fileOutput = Environment.GetEnvironmentVariable("--file-output");
        if (string.IsNullOrWhiteSpace(fileOutput))
        {
            throw new ArgumentException(nameof(fileOutput));
        }
        var timeStartString = Environment.GetEnvironmentVariable("--time-start");
        if (string.IsNullOrWhiteSpace(timeStartString))
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
    /// <summary>
    /// Метод, который получает параметры через командную строку
    /// </summary>
    /// <param name="args">Параметры командной строки</param>
    /// <returns>Возвращает объект IPConfiguration</returns>
    /// <exception cref="ArgumentException">Исключение, если в метод для параметра передается некорректное значение</exception>
    public IPConfiguration GetIPConfigurationFromCommandPrompt(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var fileLog = GetParameterValue(args, "--file-log");
        if (string.IsNullOrWhiteSpace(fileLog))
        {
            throw new ArgumentException(nameof(fileLog));
        }
        var fileOutput = GetParameterValue(args, "--file-output");

        if (string.IsNullOrWhiteSpace(fileOutput))
        {
            throw new ArgumentException(nameof(fileOutput));
        }
        var timeStartString = GetParameterTimeValue(args, "--time-start");
        if (string.IsNullOrWhiteSpace(timeStartString))
        {
            throw new ArgumentException(nameof(timeStartString));
        }
        var timeEndString = GetParameterTimeValue(args, "--time-end");
        if (string.IsNullOrWhiteSpace(timeEndString))
        {
            throw new ArgumentException(nameof(timeEndString));
        }
        var addressStart = GetParameterValue(args, "--address-start");
        var addressMask = GetParameterValue(args, "--address-mask");

        return ParseIPConfigurationData(fileLog, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
    }
    private IPConfiguration ParseIPConfigurationData
        (string fileLog, string fileOutput, string timeStartString, string timeEndString,
        string? addressStart, string? addressMask)
    {
        DateTime timeStart, timeEnd;
        CultureInfo provider = new CultureInfo("ru-RU");
        if (!DateTime.TryParseExact(timeStartString, "yyyy-MM-dd HH:mm:ss", provider, DateTimeStyles.None, out timeStart))
            throw new FormatException("Не удалось преобразовать тип данных string в DateTime");
        if (!DateTime.TryParseExact(timeEndString, "yyyy-MM-dd HH:mm:ss", provider, DateTimeStyles.None, out timeEnd))
            throw new FormatException("Не удалось преобразовать тип данных string в DateTime");

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
    private string? GetParameterValue(string[] args, string parameter)
    {
        int index = Array.IndexOf(args, parameter);
        return args[index + 1];
    }
    private string? GetParameterTimeValue(string[] args, string parameter)
    {
        int index = Array.IndexOf(args, parameter);
        return $"{args[index + 1]} {args[index + 2]}";
    }

}