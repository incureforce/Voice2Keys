using Utils;

namespace VoiceToKeys.VMs {
    class ScreenVM : UIModelBase {
        protected ScreenVM(string name) {
            Name = name;
        }

        public string Name {
            get;
            private set;
        }
    }
}
