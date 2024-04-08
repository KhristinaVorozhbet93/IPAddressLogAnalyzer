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

    /// <summary>
    /// Метод, который считывает IP-адреса с файла журнала, применяет к IP-адресам заданные парамеры и записывает IP-адреса в новый файл
    /// </summary>
    public async virtual Task WriteIPAddressesWithConfigurationsToFile()
    {
        var ipAddresses = await _iPFilesService.ReadFromFileToListAsync(_configuration.FileLog);

        ipAddresses.Sort();
        var timeAddresses = GetIPAddressesInTimeInterval(ipAddresses, _configuration.TimeStart, _configuration.TimeEnd);

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

    /// <summary>
    /// Метод, который осуществляет фильтрацию по нижней границе диапазона адресов и маске подсети
    /// </summary>
    /// <param name="ipAddresses">Список IP-адресов, в которых нужно осуществить фильтрацию по заданным параметрам</param>
    /// <param name="addressStart">Нижняя граница диапазона адресов</param>
    /// <param name="addressMask">Маска подсети, задающая верхнюю границу диапазона</param>
    /// <returns>Возвращает словарь IP-адресов с примененными конфигурациями, где key - IpAddress, value - количество обращений с данного адреса</returns>
    public Dictionary<IPAddress, int> GetRangeIPAddresses(Dictionary<IPAddress, int> ipAddresses, IPAddress addressStart, IPAddress addressMask)
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
    public List<IP> GetIPAddressesInTimeInterval(List<IP> ipAddresses, DateTime timeStart, DateTime timeEnd)
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

