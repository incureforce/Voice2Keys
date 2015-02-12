using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToKeys.BusinessLogic {
    public class GameContext : IDisposable {
        static IEnumerable<IntPtr> SafeGetProcessesBy(IEnumerable<string> processNames) {
            var gameProcessMainWindowHandleList = new List<IntPtr>();

            foreach (var process in Process.GetProcesses()) {
                try {
                    var processName = process.ProcessName;
                    var processMainWindowHandle = process.MainWindowHandle;
                    if (processNames.Contains(processName)) {
                        gameProcessMainWindowHandleList.Add(processMainWindowHandle);
                    }
                } catch {
                }
            }

            return gameProcessMainWindowHandleList;
        }

        public GameContext(Game game) {
            Game = game;
            GameCommands = game.CommandCollection;
        }

        protected SpeechRecognitionEngine SpeechRecognitionEngine {
            get;
            private set;
        }

        protected Game Game {
            get;
            private set;
        }

        public IEnumerable<GameCommand> GameCommands {
            get;
            private set;
        }

        private bool IsCurrentInputProcess() {
            var gameProcessNameCollection = Game.ProcessNameCollection;

            if (gameProcessNameCollection.Count == 0) {
                return true;
            }

            var gameProcessMainWindow = Win32.GetForegroundWindow();
            var gameProcessMainWindowHandles = SafeGetProcessesBy(gameProcessNameCollection);

            return gameProcessMainWindowHandles.Contains(gameProcessMainWindow);
        }

        public async Task LaunchAsync() {
            var recognitionResult = default(RecognitionResult);
            var recognitionResultAlternate = default(RecognizedPhrase);
            var recognitionResultAlternates = default(IEnumerable<RecognizedPhrase>);

            SpeechRecognitionEngine = new SpeechRecognitionEngine();

            await Task.Run(() => {
                SpeechRecognitionEngine.LoadGrammar(Game.NewGrammar());
                SpeechRecognitionEngine.SetInputToDefaultAudioDevice();
            });

            do {
                recognitionResult = await Task.Run(() => SpeechRecognitionEngine.Recognize());

                if (recognitionResult != null && IsCurrentInputProcess()) {
                    recognitionResultAlternates = recognitionResult.Alternates;
                    recognitionResultAlternates = recognitionResultAlternates.Where(p => p.Confidence > .94F);
                    recognitionResultAlternates = recognitionResultAlternates.OrderByDescending(p => p.Confidence);
                    recognitionResultAlternate = recognitionResultAlternates.FirstOrDefault();

                    /*
                    recognitionResultAlternates = recognitionResultAlternates.Where(p => p.Confidence >= 94);
                    recognitionResultAlternate = recognitionResultAlternates.FirstOrDefault();
                    */
                    if (recognitionResultAlternate != null) {
                        var command = CommandBy(recognitionResultAlternate.Text);
                        if (command != null) {
                            command.Call();
                        }
                    }
                }
            } while (SpeechRecognitionEngine != null);
        }4

        private GameCommand CommandBy(string commandFull) {
            return GameCommands.SingleOrDefault(p => p.Full == commandFull);
        }

        public void Dispose() {
            if (SpeechRecognitionEngine != null) {
                SpeechRecognitionEngine.Dispose();
                SpeechRecognitionEngine = null;
            }
        }
    }
}
