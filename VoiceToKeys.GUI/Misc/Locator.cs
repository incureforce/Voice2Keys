using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToKeys.VMs;

namespace VoiceToKeys.Misc {
    class Locator {
        IOC ioc;

        public Locator() {
            ioc = App.IOC;
        }

        public MainVM MainVM {
            get {
                return ioc.Get<MainVM>();
            }
        }
    }
}
