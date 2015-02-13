/* Copyright (C) 2015 Stefan Atzlesberger
 * See README.md for further informations
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Threading.Tasks;

namespace VoiceToKeys.BusinessLogic {
    public class ProfileContext : IDisposable {
        static IEnumerable<IntPtr> SafeGetProcessesBy(IEnumerable<string> processNames) {
            var profileProcessMainWindowHandleList = new List<IntPtr>();

            foreach (var process in Process.GetProcesses()) {
                try {
                    var processName = process.ProcessName;
                    var processMainWindowHandle = process.MainWindowHandle;
                    if (processNames.Contains(processName)) {
                        profileProcessMainWindowHandleList.Add(processMainWindowHandle);
                    }
                }
                catch {
                }
            }

            return profileProcessMainWindowHandleList;
        }

        public ProfileContext(Profile profile) {
            Profile = profile;
            profileCommands = profile.CommandCollection;
        }

        protected SpeechRecognitionEngine SpeechRecognitionEngine {
            get;
            private set;
        }

        protected Profile Profile {
            get;
            private set;
        }

        public IEnumerable<ProfileCommand> profileCommands {
            get;
            private set;
        }

        private bool IsCurrentInputProcess() {
            var profileProcessNameCollection = Profile.ProcessNameCollection;

            if (profileProcessNameCollection.Count == 0) {
                return true;
            }

            var profileProcessMainWindow = Win32.GetForegroundWindow();
            var profileProcessMainWindowHandles = SafeGetProcessesBy(profileProcessNameCollection);

            return profileProcessMainWindowHandles.Contains(profileProcessMainWindow);
        }

        public async Task LaunchAsync() {
            var recognitionResult = default(RecognitionResult);
            var recognitionResultAlternate = default(RecognizedPhrase);
            var recognitionResultAlternates = default(IEnumerable<RecognizedPhrase>);

            SpeechRecognitionEngine = new SpeechRecognitionEngine();

            await Task.Run(() => {
                SpeechRecognitionEngine.LoadGrammar(Profile.NewGrammar());
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
        }

        private ProfileCommand CommandBy(string commandFull) {
            return profileCommands.SingleOrDefault(p => p.Full == commandFull);
        }

        public void Dispose() {
            if (SpeechRecognitionEngine != null) {
                SpeechRecognitionEngine.Dispose();
                SpeechRecognitionEngine = null;
            }
        }
    }
}
