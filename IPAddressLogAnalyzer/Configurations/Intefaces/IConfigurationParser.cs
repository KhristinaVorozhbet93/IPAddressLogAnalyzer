namespace IPAddressLogAnalyzer.Configurations.Intefaces
{
    public interface IConfigurationParser
    {
        IPConfiguration ParseIPConfigurationData
            (string fileLog, string fileOutput, string timeStartString,
            string timeEndString, string? addressStart, string? addressMask);
    }
}
