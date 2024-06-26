﻿using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace IPAddressLogAnalyzer.Tests
{
    public class IPConfigurationTests
    {
        [Fact]
        public void Configurations_from_command_prompt_has_correct_props()
        {
            string[] args =
                ["--file-log", "assests/FileLog.txt", "--file-output",
                "assests/OutputApis.txt", "--time-start", "2024-10-14", "23:59:06", "--time-end", "2024-10-14", "23:59:07",
                "--address-start", "", "--address-mask", ""];
            Configuration config = new Configuration();

            FluentActions.Invoking(() =>
            {
                var ipConfig = config.GetIPConfigurationFromCommandPrompt(args);
            })
               .Should()
               .NotThrow();
        }

        [Theory]
        [InlineData([new string[]
        {
        "--file-log", "", "--file-output",
        "assests/OutputApis.txt", "--time-start", "2024-10-14", "23:59:06",
        "--time-end", "2024-10-14", "23:59:07",
        "--address-start", "", "--address-mask", ""
        }])]
        [InlineData([new string[]
        {
        "--file-log", "assests/FileLog.txt", "--file-output",
        "", "--time-start", "2024-10-14", "23:59:06",
        "--time-end", "2024-10-14", "23:59:07",
        "--address-start", "", "--address-mask", ""
        }])]
        [InlineData([new string[]
        {
        "--file-log", "assests/FileLog.txt", "--file-output",
        "assests/OutputApis.txt", "--time-start", "","",
        "--time-end", "2024-10-14","23:59:07", "--address-start", "", "--address-mask", ""
        }])]
        [InlineData([new string[]
        {
        "--file-log", "assests/FileLog.txt", "--file-output",
        "assests/OutputApis.txt", "--time-start", "2024-10-14", "23:59:06",
        "--time-end", "","", "--address-start", "", "--address-mask", ""
        }])]

        public void Configurations_creatioin_from_command_prompt_with_incorrect_props_is_rejected(string[] args)
        {
            Configuration config = new Configuration();

            FluentActions.Invoking(() => { 
                var ipConfig = config.GetIPConfigurationFromCommandPrompt(args); })
                .Should()
                .Throw<ArgumentException>();
        }

        [Theory]
        [InlineData([new string[]
        {
        "--file-log", "assests/FileLog.txt", "--file-output",
        "assests/OutputApis.txt", "--time-start", "2024-10","23",
        "--time-end", "2024-10-14","23:59:07", "--address-start", "", "--address-mask", ""
        }])]
        [InlineData([new string[]
        {
        "--file-log", "assests/FileLog.txt", "--file-output",
        "assests/OutputApis.txt", "--time-start", "2024-10-14","23:59:07",
        "--time-end", "2024","23", "--address-start", "", "--address-mask", ""
        }])]
        public void Configurations_creatioin_from_command_prompt_with_incorrect_date_time_is_rejected(string[] args)
        {
            Configuration config = new Configuration();

            FluentActions.Invoking(() => {
                var ipConfig = config.GetIPConfigurationFromCommandPrompt(args);
            })
                .Should()
                .Throw<FormatException>();
        }

        [Fact]
        public void Configurations_from_configure_file_has_correct_props()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var ipConfigSection = configuration
                .GetRequiredSection("IPConfiguration");
            Configuration config = new Configuration();

            
            FluentActions.Invoking(() =>
            {
                config.GetIPConfigurationFromConfigurationFile(ipConfigSection);

            })
               .Should()
               .NotThrow();
        }

        [Theory]
        [InlineData ("file_log", "")]
        [InlineData("file_output", "")]
        [InlineData("time_start", "")]
        [InlineData("time_end", "")]
        public void Configurations_creatioin_from_configure_file_with_incorrect_props_is_rejected(string section, string value)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var ipConfigSection = configuration
                .GetRequiredSection("IPConfiguration");
            ipConfigSection[section] = value;

            Configuration config = new Configuration();

            FluentActions.Invoking(() =>
            {
                config.GetIPConfigurationFromConfigurationFile(ipConfigSection);})
               .Should()
                .Throw<ArgumentException>();
        }
    }
}
