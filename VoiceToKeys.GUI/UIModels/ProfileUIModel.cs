/* Copyright (C) 2015 Stefan Atzlesberger
 * See README.md for further informations
 */

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
