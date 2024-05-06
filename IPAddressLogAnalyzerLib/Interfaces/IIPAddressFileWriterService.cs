using System.Net;

namespace IPAddressLogAnalyzer.Interfaces
{
    public interface IIPAddressFileWriterService
    {
        Task WriteToFileAsync(string filePath, Dictionary<IPAddress, int> ips, CancellationToken cancellationToken);
    }
}
