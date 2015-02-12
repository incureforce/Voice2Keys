using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using VoiceToKeys.Misc;

namespace VoiceToKeys.BusinessLogic {
    public class Game {
        static public Game TryNew(GameLibrary gameLibrary, DirectoryInfo gameDirectory) {
#if RELEASE
            try {
                return new Game(gameLibrary, gameDirectory);
            } catch (Exception ex) {
                return null;
            }
#else
            return new Game(gameLibrary, gameDirectory);
#endif
        }

        public Game(GameLibrary gameLibrary, DirectoryInfo directory) {
            var logger = new Logger(this);
            var loggerModules = logger.Modules;
            {
                loggerModules.Add(Logger.CurrentDateTimeModule);
            }

            if (directory.Exists == false) {
                directory.Create();
            }

            ProcessNameCollection = new ObservableCollection<string>();
            CommandCollection = new ObservableCollection<GameCommand>();
            Directory = directory;
            Library = gameLibrary;
            Logger = logger;
            File = new FileInfo(directory.FullName + "/Init.lua");

            // Cleanup set

            LUAInterface = new GameLUAInterface(gameLibrary.Lua, this);
        }

        internal GameLUAInterface LUAInterface {
            get;
            private set;
        }

        public ICollection<GameCommand> CommandCollection {
            get;
            private set;
        }

        public GameLibrary Library {
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

        private Choices ChoicesBy(IEnumerable<GameCommand> commands) {
            var grammarBuilders =
                from command in commands
                select GrammarBuilderBy(command);
            return new Choices(grammarBuilders.ToArray());
        }

        private GrammarBuilder GrammarBuilderBy(GameCommand command) {
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
