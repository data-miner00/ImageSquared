using System.Configuration;
using System.Data;
using System.Windows;
using Autofac;
using ImageSquared.View;

namespace ImageSquared
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application, IDisposable
    {
        private readonly IContainer container = ContainerConfig.Configure();

        private bool isDisposed;

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = this.container.Resolve<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.Dispose();

            base.OnExit(e);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.container.Dispose();
            this.isDisposed = true;
        }
    }

}
