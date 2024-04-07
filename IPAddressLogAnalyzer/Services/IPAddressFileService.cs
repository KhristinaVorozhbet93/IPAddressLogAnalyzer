using IPAddressLogAnalyzer.Entities;
using IPAddressLogAnalyzer.Interfaces;
using System.Globalization;
using System.Net;

namespace IPAddressLogAnalyzer.Services
{
    public class IPAddressFileService : IIPAddressFileService
    {
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
