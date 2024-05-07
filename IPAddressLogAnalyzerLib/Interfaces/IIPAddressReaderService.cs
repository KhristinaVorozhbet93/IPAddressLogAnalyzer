using IPAddressLogAnalyzer.Entities;

namespace IPAddressLogAnalyzer.Interfaces
{
    public interface IIPAddressReaderService
    {
        Task<List<IP>> ReadToListAsync(CancellationToken cancellationToken);
    }
}
