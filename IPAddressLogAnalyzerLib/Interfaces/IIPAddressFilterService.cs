using IPAddressLogAnalyzer.Entities;
using System.Net;

namespace IPAddressLogAnalyzer.Lib.Interfaces
{
    public interface IIPAddressFilterService
    {
        Dictionary<IPAddress, int> GetIPAddressesWithConfigurations(List<IP> ipAddresses);
        Dictionary<IPAddress, int> GetRangeIPAddresses(Dictionary<IPAddress, int> ipAddresses, IPAddress addressStart, IPAddress addressMask);
        List<IP> GetIPAddressesInTimeInterval(List<IP> ipAddresses, DateTime timeStart, DateTime timeEnd);
        Dictionary<IPAddress, int> GetIPAddressesWithCountTimeRequests(List<IP> ipAddresses);
    }
}
