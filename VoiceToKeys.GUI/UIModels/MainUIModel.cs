/* Copyright (C) 2015 Stefan Atzlesberger
 * See README.md for further informations
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils;
using VoiceToKeys.BusinessLogic;
using VoiceToKeys.Misc;
using VoiceToKeys.Models;

namespace VoiceToKeys.UIModels {
    class MainUIModel : UIModelBase {
        static public MainUIModel Instance {
            get {
                return FactoryManager.Get<MainUIModel>();
            }
        }

        ProfileLibrary profileLibrary = new ProfileLibrary(FactoryManager.Get<Logger>());

        [FactoryInjection]
        public Logger Logger {
            get;
            private set;
        }

        ICollection<ProfileUIModel> profileUIModelCollection;
        public ICollection<ProfileUIModel> ProfileUIModelCollection {
            get {
                if (profileUIModelCollection == null) {
                    profileUIModelCollection = new ObservableCollection<ProfileUIModel>();
                    ProfileUIModelCollectionLoad();
                }
                return profileUIModelCollection;
            }
        }

        ICollection<string> logMessageCollection;
        public ICollection<string> LogMessageCollection {
            get {
                if (logMessageCollection == null) {
                    var logMessageCollector = new LogMessageCollector();

                    logMessageCollection = logMessageCollector.Collection;

                    Logger.WriterCollection.Add(logMessageCollector);
                }

                return logMessageCollection;
            }
        }

        ICommand loadCommand;
        public ICommand LoadCommand {
            get {
                if (loadCommand == null) {
                    loadCommand = new UICommand<object>(p => ProfileUIModelCollectionLoad());
                }
                return loadCommand;
            }
        }

        ICommand playCommand;
        public ICommand PlayCommand {
            get {
                if (playCommand == null) {
                    playCommand = new UICommand<object>(p => Play(), p => (profileUIModel != null));
                }
                return playCommand;
            }
        }

        ICommand stopCommand;
        public ICommand StopCommand {
            get {
                if (stopCommand == null) {
                    stopCommand = new UICommand<object>(p => Stop(), p => (profileUIModel != null));
                }
                return stopCommand;
            }
        }

        ProfileUIModel profileUIModel;
        public ProfileUIModel ProfileUIModel {
            get {
                return profileUIModel;
            }
            set {
                if (IfPropertyChanged(ref profileUIModel, value)) {
                    (stopCommand as UICommand<object>).DoCanExecuteChanged();
                    (playCommand as UICommand<object>).DoCanExecuteChanged();

                    Logger.Log("Change Game to: {0}", value.Name);
                }
            }
        }

        private async void ProfileUIModelCollectionLoad() {
            Stop();

            profileLibrary.Search();
            profileUIModelCollection.Clear();

            foreach (var game in profileLibrary.Collection) {

                profileUIModelCollection.Add(new ProfileUIModel(game));
            }
        }

        ProfileContext profileContext;

        private async void Play() {
            Stop();

            await Task.Delay(1000);

            profileContext = profileUIModel.ToContext();

            await profileContext.LaunchAsync();
        }

        private void Stop() {
            if (profileContext != null) {
                profileContext.Dispose();
                profileContext = null;
            }
        }
    }
}
