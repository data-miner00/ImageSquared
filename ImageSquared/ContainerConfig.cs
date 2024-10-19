namespace ImageSquared;

using System;
using Autofac;
using ImageSquared.Option;
using ImageSquared.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

/// <summary>
/// The IoC container orchestrator.
/// </summary>
internal static class ContainerConfig
{
    private const string JsonFileName = "settings.json";

    /// <summary>
    /// Gets the configured IoC container.
    /// </summary>
    /// <returns>The IoC container.</returns>
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
