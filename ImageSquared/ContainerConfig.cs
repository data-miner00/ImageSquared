namespace ImageSquared;

using System;
using System.Windows.Media.Imaging;
using Autofac;
using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.Integrations.Repositories;
using ImageSquared.Option;
using ImageSquared.View;
using ImageSquared.ViewModel;
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
            .RegisterRepositories()
            .RegisterOpenFileDialog()
            .RegisterNamingStrategy()
            .RegisterBitmapEncoders()
            .RegisterViewModels()
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
        builder
            .Register(ctx =>
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

    private static ContainerBuilder RegisterRepositories(this ContainerBuilder builder)
    {
        builder
            .Register(ctx =>
            {
                var settings = ctx.Resolve<DefaultSettings>();

                return new FileHistoryRepository(settings.HistoryFilePath);
            })
            .As<IHistoryRepository>()
            .InstancePerDependency();

        return builder;
    }

    private static ContainerBuilder RegisterNamingStrategy(this ContainerBuilder builder)
    {
        builder
            .Register(ctx =>
            {
                var settings = ctx.Resolve<DefaultSettings>();

                var imageFormat = settings.OutputSettings.ImageFormat;
                Guard.ThrowIfDefault(imageFormat);

                var namingStrategy = settings.OutputSettings.NamingStrategy;
                Guard.ThrowIfDefault(namingStrategy);

                var prefix = settings.OutputSettings.Prefix;
                var factory = new OutputNamingStrategyFactory(imageFormat, prefix);

                return factory.Create(namingStrategy);
            })
            .As<IOutputNamingStrategy>()
            .SingleInstance();

        return builder;
    }

    private static ContainerBuilder RegisterBitmapEncoders(this ContainerBuilder builder)
    {
        var encoders = new Dictionary<ImageFormat, BitmapEncoder>
        {
            { ImageFormat.Png, new PngBitmapEncoder() },
            { ImageFormat.Bmp, new BmpBitmapEncoder() },
            { ImageFormat.Jpg, new JpegBitmapEncoder() },
            { ImageFormat.Tiff, new TiffBitmapEncoder() },
        };

        builder.RegisterInstance<IDictionary<ImageFormat, BitmapEncoder>>(encoders);

        return builder;
    }

    private static ContainerBuilder RegisterViewModels(this ContainerBuilder builder)
    {
        builder.RegisterType<ConversionViewModel>();
        builder.RegisterType<HistoryViewModel>();
        builder.RegisterType<MainViewModel>();

        return builder;
    }
}
