using IPAddressLogAnalyzer.Configurations;
using IPAddressLogAnalyzer.Configurations.Intefaces;
using IPAddressLogAnalyzer.Interfaces;
using IPAddressLogAnalyzer.Lib.Interfaces;
using IPAddressLogAnalyzer.Lib.Services;
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
            .AddScoped<IConfigurationParser, ConfigurationParser>()
            .AddScoped<IIPAddressFilterService>(provider =>
            {
                var ipServiceS = provider.GetRequiredService<IConfigurationsProvider>();
                var ipConfiguration = ipServiceS.GetIPConfiguration();
                return new IPAddressFilterService
                    (ipConfiguration.TimeStart, ipConfiguration.TimeEnd, ipConfiguration.AddressStart, ipConfiguration.AddressMask);
            })
            .AddScoped<IConfigurationsProvider>(provider =>
                new FileConfigurationsProvider(ipConfigSection, provider.GetRequiredService<IConfigurationParser>()))
            .AddScoped<IIPAddressWriterService>(provider =>
            {
                var ipServiceS = provider.GetRequiredService<IConfigurationsProvider>();
                var ipConfiguration = ipServiceS.GetIPConfiguration();
                return new IPAddressFileWriterService(ipConfiguration.FileOutput);
            })
            .AddScoped<IIPAddressReaderService>(provider =>
            {
                var ipServiceS = provider.GetRequiredService<IConfigurationsProvider>();
                var ipConfiguration = ipServiceS.GetIPConfiguration();
                var ipFiltredService = provider.GetRequiredService<IIPAddressFilterService>();
                return new IPAddressFileReaderService(ipConfiguration.FileLog, ipFiltredService);
            })
            //.AddScoped<IConfigurationsProvider>(provider =>
            //      new CommandLineConfigurationsProvider(args, provider.GetRequiredService<IConfigurationParser>()))
            // .AddScoped<IConfigurationsProvider>(provider =>
            //      new EnvironmentConfigurationsProvider(provider.GetRequiredService<IConfigurationParser>()))
            .AddScoped(provider =>
            {
                var ipAddressReaderService = provider.GetRequiredService<IIPAddressReaderService>();
                var ipAddressRWriterService = provider.GetRequiredService<IIPAddressWriterService>();
                return new IPService(ipAddressRWriterService, ipAddressReaderService);
            })
            .BuildServiceProvider();


        //кофигурационный файл, командная строка, переменные среды окружения
        var ipServiceS = serviceProvider.GetRequiredService<IConfigurationsProvider>();
        var ipConfiguration = ipServiceS.GetIPConfiguration();

        CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        var ipService = serviceProvider.GetRequiredService<IPService>();
        var ips = await ipService.GetIPAddressesAsync(cancellationToken);
        await ipService.AddIPAddressesAsync(ips, cancellationToken);
        Console.WriteLine("IP-адреса с заданными конфигурациями успешно записаны в файл");
    }
}