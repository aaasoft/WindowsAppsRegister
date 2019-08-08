using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsAppsRegister.Model;

namespace WindowsAppsRegister
{
    public class MainWindow_ViewModel : PropertyNotifyModel
    {
        public ObservableCollection<AppxPackage> AppxPackageCollection { get; set; } = new ObservableCollection<AppxPackage>();
        private int _ProgressValue;
        public int ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                _ProgressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }

        private int _ProgressMaximum;
        public int ProgressMaximum
        {
            get { return _ProgressMaximum; }
            set
            {
                _ProgressMaximum = value;
                RaisePropertyChanged(nameof(ProgressMaximum));
            }
        }
    }
}
