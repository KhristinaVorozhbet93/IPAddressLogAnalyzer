using IPAddressLogAnalyzer.Configurations;
using IPAddressLogAnalyzer.Configurations.Intefaces;
using IPAddressLogAnalyzer.Interfaces;
using IPAddressLogAnalyzer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program 
{
    static async Task Main(string[] args)
    {
        //if (args.Length == 0)
        //{
        //    Console.WriteLine("Задайте параметры для дальнейшей работы приложения ");
        //    return;
        //}

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
      
        var serviceProvider = new ServiceCollection()
            .AddScoped<IIPAddressFileWriterService, IPAddressFileWriterService>()
            .AddScoped<IIPAddressFileReaderService, IPAddressFileReaderService>()
            .AddScoped<IConfigurationsProvider, EnvironmentConfigurationsProvider>()
            .AddScoped<IConfigurationParser, ConfigurationParser>()
            .AddScoped<IConfigurationsProvider>(provider =>
            {
                return new FileConfigurationsProvider(ipConfigSection, provider.GetRequiredService<IConfigurationParser>());
            })
            .AddScoped<IConfigurationsProvider>(provider =>
            {
                return new CommandLineConfigurationsProvider(args, provider.GetRequiredService<IConfigurationParser>());
            })
            .AddScoped<IPService>()
            .BuildServiceProvider();

        //кофигурационный файл
        var ipServiceS = serviceProvider.GetRequiredService<FileConfigurationsProvider>();
        var ipConfiguration = ipServiceS.GetIPConfiguration();

        //командная строка
        //var ipServiceS = serviceProvider.GetRequiredService<CommandLineConfigurationsProvider>();
        //var ipConfiguration = ipServiceS.GetIPConfiguration();

        //переменные среды окружения
        //var ipServiceS = serviceProvider.GetRequiredService<EnvironmentConfigurationsProvider>();
        //var ipConfiguration = ipServiceS.GetIPConfiguration();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        var ipService = serviceProvider.GetRequiredService<IPService>();
        await ipService.WriteIPAddressesWithConfigurationsToFile
            (ipConfiguration.FileLog, ipConfiguration.FileOutput, ipConfiguration.TimeStart,
            ipConfiguration.TimeEnd, ipConfiguration.AddressStart, ipConfiguration.AddressMask, cancellationToken);
        Console.WriteLine("IP-адреса с заданными конфигурациями успешно записаны в файл");
    }
}