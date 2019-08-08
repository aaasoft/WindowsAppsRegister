using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAppsRegister.Model
{
    public class AppxPackage
    {
        public string Name { get; set; }
        public string Publisher { get; set; }
        public object Architecture { get; set; }
        public string ResourceId { get; set; }
        public string Version { get; set; }
        public string PackageFullName { get; set; }
        public string InstallLocation { get; set; }
        public bool IsFramework { get; set; }
        public string PackageFamilyName { get; set; }
        public string PublisherId { get; set; }
        public bool IsResourcePackage { get; set; }
        public bool IsBundle { get; set; }
        public bool IsDevelopmentMode { get; set; }
        public bool NonRemovable { get; set; }
        public bool IsPartiallyStaged { get; set; }
        public object SignatureKind { get; set; }
        public object Status { get; set; }

        public static AppxPackage GetItem(PSObject obj)
        {
            var type = typeof(AppxPackage);
            var model = new AppxPackage();
            foreach( var pi in type.GetProperties())
            {
                pi.SetValue(model, obj.Properties[pi.Name].Value);
            }
            return model;
        }
    }
}
