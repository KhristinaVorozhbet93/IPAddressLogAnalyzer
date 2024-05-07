#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
using System.ComponentModel.DataAnnotations;

namespace IPAddressLogAnalyzer
{
    public class IPConfiguration
    {
        [Required]
        public string FileLog { get; set; }

        [Required]
        public string FileOutput { get; set; }

        [Required]
        public DateTime TimeStart { get; set; }

        [Required]
        public DateTime TimeEnd { get; set; }

        public string? AddressStart { get; set; }
        public string? AddressMask { get; set; }

    }
}