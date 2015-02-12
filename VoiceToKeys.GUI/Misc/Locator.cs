using Utils;
using VoiceToKeys.VMs;

namespace VoiceToKeys.Misc {
    class Locator {
        FactoryManager factoryManager;

        public Locator() {
            factoryManager = UIModelBase.FactoryManager;
        }

        public MainVM MainVM {
            get {
                return factoryManager.Get<MainVM>();
            }
        }
    }
}
