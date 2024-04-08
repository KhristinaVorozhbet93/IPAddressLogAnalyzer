using IPAddressLogAnalyzer.Entities;
using IPAddressLogAnalyzer.Interfaces;
using System.Globalization;
using System.Net;

namespace IPAddressLogAnalyzer.Services
{
    public class IPAddressFileService : IIPAddressFileService
    {
        /// <summary>
        /// Метод, который парсит содержимое файла в списко IP-адрессов
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Исключение, если нужный файл по заданному пути не найден</exception>
        /// <exception cref="ArgumentException"> Исключение, если в метод для параметра передается некорректное значение</exception>
        public async Task<List<IP>> ReadFromFileToListAsync(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException
                    ($"Файл по заданному пути не обнаружен: {filePath}");
            }

            List<IP> ips = new List<IP>();
            CultureInfo provider = new CultureInfo("ru-RU");
            using (StreamReader reader = new StreamReader(filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
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
            throw new ArgumentException($"Не удалось найти файл: {filePath}");
        }

        /// <summary>
        /// Метод, который записывает словарь IP-адресов в файл
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="ips">Словарь IP-адресов, где key - IP-адрес, value - количество обращений с адреса </param>
        public async Task WriteToFileAsync
            (string filePath, Dictionary<IPAddress, int> ips)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);
            ArgumentNullException.ThrowIfNull(ips);
            using StreamWriter writer = new(filePath, false);
            foreach (var ip in ips)
            {
                await writer.WriteLineAsync($"{ip.Key} {ip.Value}");
            }
        }
    }
}
