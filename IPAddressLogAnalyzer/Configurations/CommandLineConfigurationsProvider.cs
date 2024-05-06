using IPAddressLogAnalyzer.Configurations.Intefaces;

namespace IPAddressLogAnalyzer.Configurations
{
    public class CommandLineConfigurationsProvider : IConfigurationsProvider
    {
        private readonly IConfigurationParser _configurationParser;
        private readonly string[] _args;
        public CommandLineConfigurationsProvider(string[] args,
            IConfigurationParser configurationParser)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(args));
            ArgumentException.ThrowIfNullOrEmpty(nameof(configurationParser));
            _args = args;
            _configurationParser = configurationParser;
        }
        /// <summary>
        /// Метод, который получает параметры через командную строку
        /// </summary>
        /// <param name="args">Параметры командной строки</param>
        /// <returns>Возвращает объект IPConfiguration</returns>
        /// <exception cref="ArgumentException">Исключение, если в метод для параметра передается некорректное значение</exception>
        public IPConfiguration GetIPConfiguration()
        {
            ArgumentNullException.ThrowIfNull(_args);
            var fileLog = GetParameterValue(_args, "--file-log");
            if (string.IsNullOrWhiteSpace(fileLog))
            {
                throw new ArgumentException(nameof(fileLog));
            }
            var fileOutput = GetParameterValue(_args, "--file-output");

            if (string.IsNullOrWhiteSpace(fileOutput))
            {
                throw new ArgumentException(nameof(fileOutput));
            }
            var timeStartString = GetParameterTimeValue(_args, "--time-start");
            if (string.IsNullOrWhiteSpace(timeStartString))
            {
                throw new ArgumentException(nameof(timeStartString));
            }
            var timeEndString = GetParameterTimeValue(_args, "--time-end");
            if (string.IsNullOrWhiteSpace(timeEndString))
            {
                throw new ArgumentException(nameof(timeEndString));
            }
            var addressStart = GetParameterValue(_args, "--address-start");
            var addressMask = GetParameterValue(_args, "--address-mask");

            return _configurationParser.ParseIPConfigurationData(fileLog, fileOutput, timeStartString, timeEndString, addressStart, addressMask);
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
}
