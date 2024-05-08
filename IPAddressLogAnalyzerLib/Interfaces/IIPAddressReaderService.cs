using System.Net;

namespace IPAddressLogAnalyzer.Interfaces
{
    public interface IIPAddressReaderService
    {
        Task<Dictionary<IPAddress,int>> ReadAsync(CancellationToken cancellationToken);
    }
}
