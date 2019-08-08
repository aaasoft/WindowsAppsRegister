using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using WindowsAppsRegister.Model;

namespace WindowsAppsRegister
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public DelegateCommand RefreshCommand { get; set; }
        public MainWindow_ViewModel ViewModel { get; } = new MainWindow_ViewModel();
        public MainWindow()
        {
            InitializeComponent();
            RefreshCommand = new DelegateCommand() { ExecuteCommand = execute_RefreshCommand };
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshCommand.Execute(null);
        }

        private async void execute_RefreshCommand(object obj)
        {
            borderLoading.Visibility = Visibility.Visible;
            ViewModel.AppxPackageCollection.Clear();
            var dirs = AppxPackage.GetDirs();
            ViewModel.ProgressMaximum = dirs.Length;
            ViewModel.ProgressValue = 0;

            foreach (var dir in dirs)
            {
                ViewModel.ProgressValue++;
                AppxPackage package = new AppxPackage();
                await Task.Run(() => package.Load(dir));
                if (
                    package.InstallLocation == null
                    || package.IsFramework
                    || package.IsResourcePackage)
                    continue;
                await package.RefreshRegisterStatusAsync();
                ViewModel.AppxPackageCollection.Add(package);
            }
            borderLoading.Visibility = Visibility.Collapsed;
        }
    }
}
