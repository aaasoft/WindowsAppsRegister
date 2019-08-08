using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsAppsRegister
{
    public class PowerShellUtils
    {
        public static async Task<Collection<PSObject>> RunPowerShellScriptAsync(string script)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(script);
                Collection<PSObject> results = null;
                await Task.Run(() => results = pipeline.Invoke());
                runspace.Close();
                return results;
            }
        }

        public static async Task<bool> GetAppxPackageRegistered(string packageId)
        {
            var document = await GetAppxPackageManifestAsync(packageId);
            return document != null;
        }

        public static async Task<XmlDocument> GetAppxPackageManifestAsync(string packageId)
        {
            var result = await RunPowerShellScriptAsync(
                $"Get-AppxPackageManifest -Package \"{packageId}\""
                );
            return result.FirstOrDefault()?.BaseObject as XmlDocument;
        }

        public static async Task<bool> AddAppxPackageAsync(string installLocation)
        {
            await RunPowerShellScriptAsync(
                $"Add-AppxPackage -register \"{installLocation}\\appxmanifest.xml\" -disabledevelopmentmode"
                );
            return await GetAppxPackageRegistered(Path.GetFileName(installLocation));
        }

        public static async Task<bool> RemoveAppxPackageAsync(string packageId)
        {
            await RunPowerShellScriptAsync(
                $"Remove-AppxPackage -package \"{packageId}\""
                );
            return !await GetAppxPackageRegistered(packageId);
        }
    }
}
