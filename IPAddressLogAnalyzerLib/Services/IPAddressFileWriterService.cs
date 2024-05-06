using IPAddressLogAnalyzer.Interfaces;
using System.Net;

namespace IPAddressLogAnalyzer.Services
{
    public class IPAddressFileWriterService : IIPAddressFileWriterService
    {
        public async Task WriteToFileAsync
            (string filePath, Dictionary<IPAddress, int> ips, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);
            ArgumentNullException.ThrowIfNull(ips);
            using StreamWriter writer = new(filePath, false);
            foreach (var ip in ips)
            {
                await writer.WriteLineAsync($"{ip.Key} {ip.Value}".ToCharArray(), cancellationToken);
            }
        }
    }
}
