using System.Net;

namespace IPAddressLogAnalyzer.Interfaces
{
    public interface IIPAddressWriterService
    {
        Task WriteAsync(Dictionary<IPAddress, int> ips, CancellationToken cancellationToken);
    }
}
