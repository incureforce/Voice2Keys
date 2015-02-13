using Utils;
using VoiceToKeys.BusinessLogic;

namespace VoiceToKeys.UIModels {
    class ProfileUIModel : UIModelBase {
        private readonly Profile Profile;

        public ProfileUIModel(Profile profile) {
            Profile = profile;

            Profile.Load();
        }

        public string Name {
            get {
                return Profile.Name;
            }
        }

        internal ProfileContext ToContext() {
            return new ProfileContext(Profile);
        }
    }
}
