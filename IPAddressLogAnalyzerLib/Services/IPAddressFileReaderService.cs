using IPAddressLogAnalyzer.Entities;
using IPAddressLogAnalyzer.Interfaces;
using System.Globalization;
using System.Net;

namespace IPAddressLogAnalyzer.Services
{
    public class IPAddressFileReaderService : IIPAddressReaderService
    {
        private readonly string _filePath;
        public IPAddressFileReaderService(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(filePath));
            _filePath = filePath;
        }
        /// <summary>
        /// Метод, который парсит содержимое файла в список IP-адрессов
        /// </summary>
        /// <param name="_filePath">Путь к файлу</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Исключение, если нужный файл по заданному пути не найден</exception>
        /// <exception cref="ArgumentException"> Исключение, если в метод для параметра передается некорректное значение</exception>
        public async Task<List<IP>> ReadToListAsync(CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(_filePath);

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException
                    ($"Файл по заданному пути не обнаружен: {_filePath}");
            }

            List<IP> ips = new List<IP>();
            CultureInfo provider = new CultureInfo("ru-RU");
            using (StreamReader reader = new StreamReader(_filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var lines = line.Split(":", 2);
                        ips.Add(new IP(IPAddress.Parse(lines[0].Trim()),
                            DateTime.ParseExact(lines[1], "yyyy-MM-dd HH:mm:ss", provider)));
                    }
                }
                return ips;
            }
            throw new ArgumentException($"Не удалось найти файл: {_filePath}");
        }
    }
}
