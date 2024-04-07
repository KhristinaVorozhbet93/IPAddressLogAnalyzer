using IPAddressLogAnalyzer;
using IPAddressLogAnalyzer.Entities;
using IPAddressLogAnalyzer.Interfaces;
using System.Net;

public class IPService
{
    private readonly IIPAddressFileService _iPFilesService;
    private readonly IPConfiguration _configuration;

    public IPService(IIPAddressFileService iPFilesService, IPConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(nameof(iPFilesService));
        ArgumentNullException.ThrowIfNull(nameof(configuration));
        _iPFilesService = iPFilesService;
        _configuration = configuration;
    }

    public async virtual Task WriteIPAddressesWithConfigurationsToFile()
    {
        var ipAddresses = await _iPFilesService.ReadFromFileToListAsync(_configuration.FileLog);

        ipAddresses.Sort();
        var timeAddresses = GetIPAddressesInTimeInterval(ipAddresses);

        var countTimeRequestIPAddresses = GetIPAddressesWithCountTimeRequests(timeAddresses);

        if (!string.IsNullOrEmpty(_configuration.AddressStart) && !string.IsNullOrEmpty(_configuration.AddressMask))
        {
            var filtredAddresses = GetRangeIPAddresses
                (countTimeRequestIPAddresses, IPAddress.Parse(_configuration.AddressStart), IPAddress.Parse(_configuration.AddressMask));
            await _iPFilesService.WriteToFileAsync(_configuration.FileOutput, filtredAddresses);
            return;
        }
        await _iPFilesService.WriteToFileAsync(_configuration.FileOutput, countTimeRequestIPAddresses);
    }

    public Dictionary<IPAddress, int> GetRangeIPAddresses(Dictionary<IPAddress, int> ipAddresses, IPAddress addressStart, IPAddress addressMask)
    {
        Dictionary<IPAddress, int> filteredIPAddresses = new Dictionary<IPAddress, int>();
        foreach (var ip in ipAddresses)
        {
            if (IsIPAddressInRange
                (ip.Key, addressStart,addressMask))
            {
                filteredIPAddresses.Add(ip.Key, ip.Value);
            }
        }
        return filteredIPAddresses;
    }

    public List<IP> GetIPAddressesInTimeInterval(List<IP> ipAddresses)
    {
        return ipAddresses.Where(ip =>
                ip.TimeRequest <= _configuration.TimeEnd &&
                ip.TimeRequest >= _configuration.TimeStart)
                .ToList();
    }

    public Dictionary<IPAddress, int> GetIPAddressesWithCountTimeRequests(List<IP> ipAddresses)
    {
        return ipAddresses
                .GroupBy(ip => ip.Address)
                .ToDictionary(group => group.Key, group => group.Count());
    }

    private bool IsIPAddressInRange(IPAddress ipAddress, IPAddress addressStart, IPAddress addressMask)
    {
        byte[] ipBytes = ipAddress.GetAddressBytes();
        byte[] startBytes = addressStart.GetAddressBytes();
        byte[] maskBytes = addressMask.GetAddressBytes();
        for (int i = 0; i < ipBytes.Length; i++)
        {
            if ((ipBytes[i] & maskBytes[i]) != (startBytes[i] & maskBytes[i]))
            {
                return false;
            }
        }
        return true;
    }
}

