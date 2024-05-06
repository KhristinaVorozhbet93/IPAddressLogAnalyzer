using IPAddressLogAnalyzer.Configurations.Intefaces;

public class EnvironmentConfigurationsProvider : IConfigurationsProvider 
{
    private readonly IConfigurationParser _configurationParser;
    public EnvironmentConfigurationsProvider(IConfigurationParser configurationParser)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(configurationParser));
        _configurationParser = configurationParser;
    }
    /// <summary>
    /// Метод, который получает параметры через переменные среды окружения
    /// </summary>
    /// <returns>Возвращает объект IPConfiguration</returns>
    /// <exception cref="ArgumentException">Исключение, если в метод для параметра передается некорректное значение</exception>
    public IPConfiguration GetIPConfiguration()
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

        return _configurationParser.ParseIPConfigurationData(fileLog!, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
    }
}