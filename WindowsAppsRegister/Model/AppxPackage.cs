using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace WindowsAppsRegister.Model
{
    public class AppxPackage
    {
        public const string WINDOWS_APP_FOLDER = @"C:\Program Files\WindowsApps";
        public const string APPX_MANIFEST_XML_FILE = "AppxManifest.xml";

        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Version { get; set; }
        public string PackageFullName { get; set; }
        public string InstallLocation { get; set; }
        public string Id => Path.GetFileName(InstallLocation);
        public bool IsFramework { get; set; }
        public bool IsResourcePackage { get; set; }
        public string Logo { get; set; }
        public string LogoFullPath => Logo == null ? null : Path.Combine(InstallLocation, Logo);
        public bool IsRegistered { get; set; }

        private bool isWorking = false;
        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand UnregisterCommand { get; set; }

        public static string[] GetDirs()
        {
            return Directory.GetDirectories(WINDOWS_APP_FOLDER);
        }

        public AppxPackage()
        {
            RegisterCommand = new DelegateCommand()
            {
                ExecuteCommand = execute_RegisterCommand,
                CanExecuteCommand = _ => !IsRegistered && !isWorking
            };
            UnregisterCommand = new DelegateCommand()
            {
                ExecuteCommand = execute_UnregisterCommand,
                CanExecuteCommand = _ => IsRegistered && !isWorking
            };
        }

        public void Load(string dir)
        {
            var appxManifestFile = Path.Combine(dir, APPX_MANIFEST_XML_FILE);
            if (!File.Exists(appxManifestFile))
                return;
            InstallLocation = dir;
            XmlDocument document = new XmlDocument();
            document.Load(appxManifestFile);

            var isFramework = document.SelectSingleNode("/*[name()='Package']/*[name()='Properties']/*[name()='Framework']")?.InnerText;
            IsFramework = isFramework != null && bool.Parse(isFramework);
            var isResourcePackage = document.SelectSingleNode("/*[name()='Package']/*[name()='Properties']/*[name()='ResourcePackage']")?.InnerText;
            IsResourcePackage = isResourcePackage != null && bool.Parse(isResourcePackage);

            var displayName = document.SelectSingleNode("/*[name()='Package']/*[name()='Properties']/*[name()='DisplayName']")?.InnerText;
            if (!displayName.StartsWith("ms-resource:"))
                Name = displayName;
            if (string.IsNullOrEmpty(Name))
                Name = document.SelectSingleNode("/*[name()='Package']/*[name()='Identity']")?.Attributes["Name"]?.InnerText;
            Version = document.SelectSingleNode("/*[name()='Package']/*[name()='Identity']")?.Attributes["Version"]?.InnerText;
            Publisher = document.SelectSingleNode("/*[name()='Package']/*[name()='Properties']/*[name()='PublisherDisplayName']")?.InnerText;
            var logo = document.SelectSingleNode("/*[name()='Package']/*[name()='Properties']/*[name()='Logo']")?.InnerText;
            Logo = logo;
            if (!File.Exists(LogoFullPath))
            {
                var part2Array = new[] { ".scale-100", ".scale-200", ".scale-400" };
                foreach (var part2 in part2Array)
                {
                    var part0 = Path.GetDirectoryName(logo);
                    var part1 = Path.GetFileNameWithoutExtension(logo);
                    var part3 = Path.GetExtension(logo);
                    Logo = Path.Combine(part0, string.Join(string.Empty, new[] { part1, part2, part3 }));
                    if (File.Exists(LogoFullPath))
                        break;
                }
            }
            if (!File.Exists(LogoFullPath))
                Logo = null;
        }

        public async Task RefreshRegisterStatusAsync()
        {
            IsRegistered = await PowerShellUtils.GetAppxPackageRegistered(Id);
        }

        private async void execute_UnregisterCommand(object obj)
        {
            isWorking = true;
            RegisterCommand.RaiseCanExecuteChanged();
            UnregisterCommand.RaiseCanExecuteChanged();
            var result = await PowerShellUtils.UnregisterAppxPackageAsync(Id);
            await RefreshRegisterStatusAsync();
            if (result)
                MessageBox.Show("反注册成功！", nameof(WindowsAppsRegister), MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("反注册失败！", nameof(WindowsAppsRegister), MessageBoxButton.OK, MessageBoxImage.Warning);
            isWorking = false;
            RegisterCommand.RaiseCanExecuteChanged();
            UnregisterCommand.RaiseCanExecuteChanged();
        }

        private async void execute_RegisterCommand(object obj)
        {
            isWorking = true;
            RegisterCommand.RaiseCanExecuteChanged();
            UnregisterCommand.RaiseCanExecuteChanged();
            var result = await PowerShellUtils.RegisterAppxPackageAsync(InstallLocation);
            await RefreshRegisterStatusAsync();
            if (result)
                MessageBox.Show("注册成功！", nameof(WindowsAppsRegister), MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("注册失败！", nameof(WindowsAppsRegister), MessageBoxButton.OK, MessageBoxImage.Warning);
            isWorking = false;
            RegisterCommand.RaiseCanExecuteChanged();
            UnregisterCommand.RaiseCanExecuteChanged();
        }
    }
}
