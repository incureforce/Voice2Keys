using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using VoiceToKeys.Misc;

namespace VoiceToKeys.BusinessLogic {
    public class Profile {
        static public Profile TryNew(ProfileLibrary profileLibrary, DirectoryInfo profileDirectory) {
#if RELEASE
            try {
                return new profile(profileLibrary, profileDirectory);
            } catch (Exception ex) {
                return null;
            }
#else
            return new Profile(profileLibrary, profileDirectory);
#endif
        }

        public Profile(ProfileLibrary profileLibrary, DirectoryInfo directory) {
            var logger = new Logger(this);
            var loggerModules = logger.Modules;
            {
                loggerModules.Add(Logger.CurrentDateTimeModule);
            }

            if (directory.Exists == false) {
                directory.Create();
            }

            ProcessNameCollection = new ObservableCollection<string>();
            CommandCollection = new ObservableCollection<ProfileCommand>();
            Directory = directory;
            Library = profileLibrary;
            Logger = logger;
            File = new FileInfo(directory.FullName + "/Init.lua");

            // Cleanup set

            LUAInterface = new ProfileLUAInterface(profileLibrary.Lua, this);
        }

        internal ProfileLUAInterface LUAInterface {
            get;
            private set;
        }

        public ICollection<ProfileCommand> CommandCollection {
            get;
            private set;
        }

        public ProfileLibrary Library {
            get;
            private set;
        }

        public Logger Logger {
            get;
            private set;
        }

        public ICollection<string> ProcessNameCollection {
            get;
            private set;
        }

        public DirectoryInfo Directory {
            get;
            private set;
        }

        public FileInfo File {
            get;
            private set;
        }

        public string Name {
            get {
                return Directory.Name;
            }
        }

        public Grammar NewGrammar() {
            var choices = ChoicesBy(CommandCollection.CommandsBy(null));
            var grammarBuilder = new GrammarBuilder(choices);

            return new Grammar(grammarBuilder);
        }

        private Choices ChoicesBy(IEnumerable<ProfileCommand> commands) {
            var grammarBuilders =
                from command in commands
                select GrammarBuilderBy(command);
            return new Choices(grammarBuilders.ToArray());
        }

        private GrammarBuilder GrammarBuilderBy(ProfileCommand command) {
            var grammarBuilder = new GrammarBuilder(command.Text);
            var grammarBuilderChoices = ChoicesBy(command.Commands);

            if (command.FunctionHandler != null) {
                grammarBuilderChoices.Add(new GrammarBuilder());
            }

            grammarBuilder.Append(grammarBuilderChoices);

            return grammarBuilder;
        }

        public void Load() {
            CommandCollection.Clear();
            ProcessNameCollection.Clear();

            if (File.Exists) {
                var lua = Library.Lua;
                using (var streamReader = File.OpenText()) {
                    LUAInterface.ResetExecute(streamReader);
                }
            }
        }
    }
}
