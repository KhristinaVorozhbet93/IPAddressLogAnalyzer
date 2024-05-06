using IPAddressLogAnalyzer.Entities;

namespace IPAddressLogAnalyzer.Interfaces
{
    public interface IIPAddressFileReaderService
    {
        Task<List<IP>> ReadFromFileToListAsync(string filePath, CancellationToken cancellationToken);
    }
}
