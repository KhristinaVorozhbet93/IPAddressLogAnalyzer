using IPAddressLogAnalyzer.Entities;
using System.Net;

namespace IPAddressLogAnalyzer.Interfaces
{
    public interface IIPAddressFileService
    {
        Task<List<IP>> ReadFromFileToListAsync(string filePath);
        Task WriteToFileAsync(string filePath, Dictionary<IPAddress, int> ips);
    }
}
