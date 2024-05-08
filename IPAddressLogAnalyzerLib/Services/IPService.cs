using IPAddressLogAnalyzer.Interfaces;
using System.Net;

public class IPService
{
    private readonly IIPAddressWriterService _iPFileWriterService;
    private readonly IIPAddressReaderService _iPFileReaderService;

    public IPService(IIPAddressWriterService iPFileWriterService,
        IIPAddressReaderService iPFileReaderService)
    {
        ArgumentNullException.ThrowIfNull(nameof(iPFileWriterService));
        ArgumentNullException.ThrowIfNull(nameof(iPFileReaderService));
        _iPFileWriterService = iPFileWriterService;
        _iPFileReaderService = iPFileReaderService;
    }

    public async virtual Task<Dictionary<IPAddress,int>> GetIPAddressesAsync(CancellationToken cancellationToken)
    {
        return await _iPFileReaderService.ReadAsync(cancellationToken);
    }

    public async virtual Task AddIPAddressesAsync(Dictionary<IPAddress, int> ips, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(ips));
        await _iPFileWriterService.WriteAsync(ips, cancellationToken);
    }
}

