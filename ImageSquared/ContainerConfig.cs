using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Configuration;
using ImageSquared.Option;
using ImageSquared.View;
using Microsoft.Extensions.Configuration;

namespace ImageSquared
{
    internal static class ContainerConfig
    {
        private const string JsonFileName = "settings.json";

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterSettingsFile();

            builder.RegisterType<MainWindow>().SingleInstance();

            return builder.Build();
        }

        private static ContainerBuilder RegisterSettingsFile(this ContainerBuilder builder)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(JsonFileName);

            var config = configBuilder.Build();

            var defaultSettings = config.GetSection(nameof(DefaultSettings)).Get<DefaultSettings>()
                ?? throw new InvalidOperationException("The settings file is missing.");

            builder.RegisterInstance(defaultSettings);

            return builder;
        }
    }
}
