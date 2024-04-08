using IPAddressLogAnalyzer;
using IPAddressLogAnalyzer.Interfaces;
using IPAddressLogAnalyzer.Services;
using Microsoft.Extensions.Configuration;

class Program : Configuration
{
    static async Task Main(string[] args)
    {
        Configuration config = new Configuration();
        //if (args.Length == 0)
        //{
        //    Console.WriteLine("Задайте параметры для дальнейшей работы приложения ");
        //    return;
        //}
        ////Командная строка
        // var ipConfiguration = config.GetIPConfigurationFromCommandPrompt(args);

        //Переменные среды окружения
        //var ipConfiguration = config.GetIPConfigurationFromEnvironmentVariable();

        //Файл конфигурации 
        var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        var ipConfigSection = configuration
            .GetRequiredSection("IPConfiguration");

        if (ipConfigSection is null)
        {
            throw new InvalidOperationException("IpConfig не заданы");
        }

        var ipConfiguration = config.GetIPConfigurationFromConfigurationFile(ipConfigSection);

        try
        {
            IIPAddressFileService ipFilesService = new IPAddressFileService();
            IPService ipService = new(ipFilesService, ipConfiguration);
            await ipService.WriteIPAddressesWithConfigurationsToFile();
            Console.WriteLine("IP-адреса с заданными конфигурациями успешно записаны в файл");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Не удалось найти файл по заданному пути");
        }
    } 
}