using IPAddressLogAnalyzer.Configurations.Intefaces;
using System.Globalization;

namespace IPAddressLogAnalyzer.Configurations
{
    public class ConfigurationParser : IConfigurationParser
    {
        public IPConfiguration ParseIPConfigurationData
            (string fileLog, string fileOutput, string timeStartString,
            string timeEndString, string? addressStart, string? addressMask)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(fileLog));
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(fileOutput));
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(timeStartString));
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(timeEndString));

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
    }
}
