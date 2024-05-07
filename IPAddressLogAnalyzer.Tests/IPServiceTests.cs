using FluentAssertions;
using IPAddressLogAnalyzer.Configurations;
using IPAddressLogAnalyzer.Services;
using System.Net;

namespace IPAddressLogAnalyzer.Tests
{
    public class IPServiceTests
    {
        private readonly IPAddressFileReaderService _iPAddressFileReaderService;
        private readonly IPAddressFileWriterService _iPAddressFileWriterService;
        private readonly IPConfiguration _ipConfiguration;
        private readonly IPService _ipService;

        public IPServiceTests()
        {
            _ipConfiguration = new()
            {
                FileLog = "assests/FileLog.txt",
                FileOutput = "assests/OutputIPs.txt",
                AddressStart = "192.168.1.1",
                AddressMask = "255.255.255.0",
                TimeStart = DateTime.Parse("2024-10-14 23:59:06"),
                TimeEnd = DateTime.Parse("2024-10-15 23:59:13")
            };

            _iPAddressFileReaderService = new IPAddressFileReaderService(_ipConfiguration.FileLog);
            _iPAddressFileWriterService = new IPAddressFileWriterService(_ipConfiguration.FileOutput);
           
            _ipService = new(_iPAddressFileWriterService, _iPAddressFileReaderService);
        }

        [Fact]
        public async Task Filtred_addresses_correctrly_write_to_file()
        {
            Dictionary<string, string> ips = new Dictionary<string, string>();

            await _ipService.WriteIPAddressesWithConfigurationsToFile
                (_ipConfiguration.TimeStart, 
                _ipConfiguration.TimeEnd, _ipConfiguration.AddressStart, _ipConfiguration.AddressMask, default);
            using (StreamReader reader = new StreamReader(_ipConfiguration.FileOutput))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var lines = line.Split(" ");
                        ips.Add(lines[0], lines[1]);
                    }
                }
            }

            ips.Should().NotBeNull();
            ips.Should().ContainSingle();
            ips.Should().ContainKey("192.168.1.1");
            ips.Should().ContainValue("1");

        }

        [Fact]
        public async Task GetIPAddressesInTimeInterval_should_be_return_single_value_and_contain_ip_192_168_1_4()
        {
            var ipAddresses = await _iPAddressFileReaderService.ReadToListAsync(default);
            var filtredIPAddresses =
                _ipService.GetIPAddressesInTimeInterval(ipAddresses, DateTime.Parse("2024-10-14 23:59:06"),
                DateTime.Parse("2024-10-14 23:59:07"));

            filtredIPAddresses.Should().NotBeNull();
            filtredIPAddresses.Should().ContainSingle();
            filtredIPAddresses.Should().Contain(ip => ip.Address.Equals(IPAddress.Parse("5.227.242.73")));
        }

        [Fact]
        public async Task GetIPAddressesWithCountTimeRequests_should_be_return_dictionary_which_contain_ip_5_227_242_72_and_count_time_requests_2()
        {
            var ipAddresses = await _iPAddressFileReaderService.ReadToListAsync(default);
            var filtredIPAddresses = _ipService.GetIPAddressesWithCountTimeRequests(ipAddresses);

            filtredIPAddresses.Should().NotBeNull();
            filtredIPAddresses.Should().ContainKey(IPAddress.Parse("5.227.242.72"));
            filtredIPAddresses.Should().ContainValue(2);
        }

        [Fact]
        public async Task GetRangeIPAddresses_should_be_return__dictionary_which_contain_ip_192_168_1_1_and_count_time_requests_1()
        {
            var ipAddresses = await _iPAddressFileReaderService.ReadToListAsync(default);
            var ipAddressesDictionary =
                _ipService.GetIPAddressesWithCountTimeRequests(ipAddresses);
            var filtredIPAddresses = _ipService.GetRangeIPAddresses
                (ipAddressesDictionary, IPAddress.Parse(_ipConfiguration.AddressStart), IPAddress.Parse(_ipConfiguration.AddressMask));

            filtredIPAddresses.Should().NotBeNull();
            filtredIPAddresses.Should().ContainSingle();
            filtredIPAddresses.Should().ContainKey(IPAddress.Parse("192.168.1.1"));
            filtredIPAddresses.Should().ContainValue(1);
        }
    }
}