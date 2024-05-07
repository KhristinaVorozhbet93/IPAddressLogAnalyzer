using System.Net;

namespace IPAddressLogAnalyzer.Entities
{
    public class IP : IComparable<IP>
    {
        private IPAddress _address;
        private DateTime _timeRequest;

        public IP(IPAddress address, DateTime timeRequest)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(address));
            _address = address;
            _timeRequest = timeRequest;
        }
        public IPAddress Address
        {
            get
            {
                return _address;
            }
            init
            {
                ArgumentException.ThrowIfNullOrEmpty(nameof(value));
                _address = value;
            }
        }

        public DateTime TimeRequest
        {
            get
            {
                return _timeRequest;
            }
            init
            {
                _timeRequest = value;
            }
        }

        public int CompareTo(IP? other)
        {
            if (other is IP iPAddress) return TimeRequest.CompareTo(iPAddress.TimeRequest);
            throw new ArgumentException("Некорректное значение параметра");
        }
    }
}