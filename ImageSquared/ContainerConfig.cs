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
using Microsoft.Win32;

namespace ImageSquared
{
    internal static class ContainerConfig
    {
        private const string JsonFileName = "settings.json";

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterSettingsFile()
                .RegisterOpenFileDialog()
                .RegisterWindows();

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

            var openFileDialogSettings = config.GetSection(nameof(OpenFileDialogSettings)).Get<OpenFileDialogSettings>()
                ?? throw new InvalidOperationException("The setting file is missing.");

            builder.RegisterInstance(openFileDialogSettings);

            return builder;
        }

        private static ContainerBuilder RegisterWindows(this ContainerBuilder builder)
        {
            builder.RegisterType<MainWindow>().SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterOpenFileDialog(this ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var settings = ctx.Resolve<OpenFileDialogSettings>();

                return new OpenFileDialog
                {
                    Filter = settings.Filter,
                    Title = settings.Title,
                    Multiselect = settings.Multiselect,
                };
            })
                .InstancePerDependency();

            return builder;
        }
    }
}
