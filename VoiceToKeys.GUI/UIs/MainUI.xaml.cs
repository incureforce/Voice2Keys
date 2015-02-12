using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoiceToKeys.Misc;

namespace VoiceToKeys.UIs {
    /// <summary>
    /// Interaction logic for MainUI.xaml
    /// </summary>
    public partial class MainUI {
        public MainUI() {
            InitializeComponent();
        }

        private async void SayIt() {
            using (var ss = new SpeechSynthesizer()) {
                ss.SetOutputToDefaultAudioDevice();
                ss.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);

                ss.Speak("Shield enabled, Engine Power at " + 55 + "%");
            }
        }
    }
}
