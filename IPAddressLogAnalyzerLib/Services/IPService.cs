using IPAddressLogAnalyzer.Entities;
using IPAddressLogAnalyzer.Interfaces;
using System.Net;

public class IPService
{
    private readonly IIPAddressFileWriterService _iPFileWriterService;
    private readonly IIPAddressFileReaderService _iPFileReaderService;

    public IPService(IIPAddressFileWriterService iPFileWriterService,
        IIPAddressFileReaderService iPFileReaderService)
    {
        ArgumentNullException.ThrowIfNull(nameof(iPFileWriterService));
        ArgumentNullException.ThrowIfNull(nameof(iPFileReaderService));
        _iPFileWriterService = iPFileWriterService;
        _iPFileReaderService = iPFileReaderService;
    }

    /// <summary>
    /// Метод, который считывает IP-адреса с файла журнала, применяет к IP-адресам заданные парамеры и записывает IP-адреса в новый файл
    /// </summary>
    public async virtual Task WriteIPAddressesWithConfigurationsToFile
        (string fileLog, string fileOutput, DateTime timeStart, DateTime timeEnd, string? addressStart, string? addressMask)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(fileLog));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(fileOutput));

        var ipAddresses = await _iPFileReaderService.ReadFromFileToListAsync(fileLog);

        ipAddresses.Sort();
        var timeAddresses = GetIPAddressesInTimeInterval(ipAddresses, timeStart, timeEnd);

        var countTimeRequestIPAddresses = GetIPAddressesWithCountTimeRequests(timeAddresses);

        if (!string.IsNullOrEmpty(addressStart) && !string.IsNullOrEmpty(addressMask))
        {
            var filtredAddresses = GetRangeIPAddresses
                (countTimeRequestIPAddresses, IPAddress.Parse(addressStart), IPAddress.Parse(addressMask));
            await _iPFileWriterService.WriteToFileAsync(fileOutput, filtredAddresses);
            return;
        }
        await _iPFileWriterService.WriteToFileAsync(fileOutput, countTimeRequestIPAddresses);
    }

    /// <summary>
    /// Метод, который осуществляет фильтрацию по нижней границе диапазона адресов и маске подсети
    /// </summary>
    /// <param name="ipAddresses">Список IP-адресов, в которых нужно осуществить фильтрацию по заданным параметрам</param>
    /// <param name="addressStart">Нижняя граница диапазона адресов</param>
    /// <param name="addressMask">Маска подсети, задающая верхнюю границу диапазона</param>
    /// <returns>Возвращает словарь IP-адресов с примененными конфигурациями, где key - IpAddress, value - количество обращений с данного адреса</returns>
    public virtual Dictionary<IPAddress, int> GetRangeIPAddresses(Dictionary<IPAddress, int> ipAddresses, IPAddress addressStart, IPAddress addressMask)
    {
        Dictionary<IPAddress, int> filteredIPAddresses = new Dictionary<IPAddress, int>();
        foreach (var ip in ipAddresses)
        {
            if (IsIPAddressInRange
                (ip.Key, addressStart, addressMask))
            {
                filteredIPAddresses.Add(ip.Key, ip.Value);
            }
        }
        return filteredIPAddresses;
    }

    /// <summary>
    /// Метод, который фильтурет IP-адреса в определенном диапазоне времени
    /// </summary>
    /// <param name="ipAddresses">Список IP-адресов, в которых нужно осуществить фильтрацию по заданным параметрам</param>
    /// <param name="timeStart">Дата и время начала</param>
    /// <param name="timeEnd">Дата и время окончания</param>
    /// <returns>Возвращает список IP-адресов с примененными конфигурациями</returns>
    public virtual List<IP> GetIPAddressesInTimeInterval(List<IP> ipAddresses, DateTime timeStart, DateTime timeEnd)
    {
        return ipAddresses.Where(ip =>
                ip.TimeRequest <= timeEnd &&
                ip.TimeRequest >= timeStart)
                .ToList();
    }

    /// <summary>
    ///  Метод, который считает количество обращений с каждого IP-адреса
    /// </summary>
    /// <param name="ipAddresses">Список IP-адресов, в которых нужно осуществить фильтрацию по заданным параметрам</param>
    /// <returns>Возвращает словарь IP-адресов с примененными конфигурациями, где key - IpAddress, value - количество обращений с данного адреса</returns>
    public virtual Dictionary<IPAddress, int> GetIPAddressesWithCountTimeRequests(List<IP> ipAddresses)
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

        if (ipBytes.Length != startBytes.Length || startBytes.Length != maskBytes.Length)
        {
            return false; 
        }

        for (int i = 0; i < ipBytes.Length; i++)
        {
            if ((ipBytes[i] & maskBytes[i]) < (startBytes[i] & maskBytes[i]))
            {
                return false; 
            }

            if ((ipBytes[i] & maskBytes[i]) > ((startBytes[i] & maskBytes[i]) + (255 - maskBytes[i])))
            {
                return false; 
            }
        }

        return true;
    }
}

