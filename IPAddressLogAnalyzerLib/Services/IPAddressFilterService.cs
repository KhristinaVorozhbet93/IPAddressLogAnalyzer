using IPAddressLogAnalyzer.Entities;
using IPAddressLogAnalyzer.Lib.Interfaces;
using System.Net;
namespace IPAddressLogAnalyzer.Lib.Services
{
    public class IPAddressFilterService : IIPAddressFilterService
    {
        private readonly DateTime _timeStart;
        private readonly DateTime _timeEnd;
        private readonly string? _addressStart;
        private readonly string? _addressMask;
        public IPAddressFilterService(DateTime timeStart, DateTime timeEnd,
            string? addressStart, string? addressMask)
        {
            _timeStart = timeStart;
            _timeEnd = timeEnd;
            _addressStart = addressStart;
            _addressMask = addressMask;
        }

        public Dictionary<IPAddress, int> GetIPAddressesWithConfigurations(List<IP> ipAddresses)
        {
            ipAddresses.Sort();
            var timeAddresses = GetIPAddressesInTimeInterval(ipAddresses, _timeStart, _timeEnd);

            var countTimeRequestIPAddresses = GetIPAddressesWithCountTimeRequests(timeAddresses);

            if (!string.IsNullOrEmpty(_addressStart) && !string.IsNullOrEmpty(_addressMask))
            {
                var filtredAddresses = GetRangeIPAddresses
                    (countTimeRequestIPAddresses, IPAddress.Parse(_addressStart), IPAddress.Parse(_addressMask));
                return filtredAddresses;
            }
            return countTimeRequestIPAddresses;
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
}