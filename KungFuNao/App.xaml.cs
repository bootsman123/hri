using KungFuNao.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

//StartupUri="MainWindow.xaml"

namespace KungFuNao
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;
        private MainWindowViewModel mainWindowViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.mainWindow = new MainWindow();
            /*
            this.mainWindowViewModel = new MainWindowViewModel();

            this.mainWindow.Closed += this.mainWindowViewModel.OnWindowClosed;

            this.mainWindow.DataContext = this.mainWindowViewModel;
             * */
            this.mainWindow.Show();
        }
    }
}
