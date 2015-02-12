using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VoiceToKeys.Misc;
using VoiceToKeys.VMs;

namespace VoiceToKeys {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static public readonly IOC IOC = new IOC();

        public App() {
            SetupSimpleIOC();
        }

        private static void SetupSimpleIOC() {
            var logger = Logger.Default;
            var loggerModules = logger.Modules; {
                loggerModules.Add(Logger.CurrentMethodModule);
                loggerModules.Add(Logger.CurrentDateTimeModule);
            }
            IOC.Set(() => logger);

            var mainVM = new MainVM();
            IOC.Set(() => mainVM);
        }
    }
}
