using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WindowsAppsRegister.Model;

namespace WindowsAppsRegister
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<AppxPackage> AppxPackageCollection { get; set; } = new ObservableCollection<AppxPackage>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void RunPowerShellScript(string script, Action<Collection<PSObject>> handler)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(script);
                Collection<PSObject> results = pipeline.Invoke();
                handler(results);
                runspace.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppxPackageCollection.Clear();
            RunPowerShellScript("Get-AppxPackage", items =>
             {
                 foreach(var item in items)
                 {
                     var package = AppxPackage.GetItem(item);
                     if (package.IsFramework || package.NonRemovable)
                         continue;

                     RunPowerShellScript($"Get-AppxPackageManifest -Package \"{package.PackageFullName}\"", manifest =>
                      {
                          var document = (XmlDocument)manifest[0].BaseObject;
                      });
                     AppxPackageCollection.Add(package);
                 }
             });            
        }
    }
}
