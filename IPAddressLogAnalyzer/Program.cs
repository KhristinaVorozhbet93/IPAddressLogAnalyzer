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
            .AddScoped<IConfigurationParser, ConfigurationParser>()
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
                return new IPAddressFileReaderService(ipConfiguration.FileLog);
            })
            //.AddScoped<IConfigurationsProvider>(provider =>
            //      new CommandLineConfigurationsProvider(args, provider.GetRequiredService<IConfigurationParser>()))
            // .AddScoped<IConfigurationsProvider>(provider =>
            //      new EnvironmentConfigurationsProvider(provider.GetRequiredService<IConfigurationParser>()))
            .AddScoped<IPService>()
            .BuildServiceProvider();


        //кофигурационный файл, командная строка, переменные среды окружения
        var ipServiceS = serviceProvider.GetRequiredService<IConfigurationsProvider>();
        var ipConfiguration = ipServiceS.GetIPConfiguration();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        var ipService = serviceProvider.GetRequiredService<IPService>();
        await ipService.WriteIPAddressesWithConfigurations
            (ipConfiguration.TimeStart, ipConfiguration.TimeEnd, ipConfiguration.AddressStart, ipConfiguration.AddressMask, cancellationToken);
        Console.WriteLine("IP-адреса с заданными конфигурациями успешно записаны в файл");
    }
}