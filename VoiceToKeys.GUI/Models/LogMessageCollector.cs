using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace VoiceToKeys.Models {
    class LogMessageCollector : TextWriter {
        ObservableCollection<string> collection = new ObservableCollection<string>();

        public override void WriteLine(string value) {
            collection.Add(value);
        }

        public override void Write(string value) {
            collection.Add(value);
        }

        public ICollection<string> Collection {
            get {
                return collection;
            }
        }

        public override Encoding Encoding {
            get { return Encoding.UTF8; }
        }
    }
}
