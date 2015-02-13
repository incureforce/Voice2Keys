using System.Windows;
using Utils;
using VoiceToKeys.Misc;
using VoiceToKeys.UIModels;

namespace VoiceToKeys {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static public readonly FactoryManager factoryManager = UIModelBase.FactoryManager;

        public App() {
            SetupSimpleIOC();
        }

        private static void SetupSimpleIOC() {
            var logger = Logger.Default;
            var loggerModules = logger.Modules;
            {
                loggerModules.Add(Logger.CurrentMethodModule);
                loggerModules.Add(Logger.CurrentDateTimeModule);
            }

            factoryManager.Set(() => logger);

            var mainUIModel = new MainUIModel();

            factoryManager.Set(() => mainUIModel);
        }
    }
}
