using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using VoiceToKeys.Misc;

namespace VoiceToKeys.VMs {
    class LogVM {
        public LogVM(string message) {
            Message = message;
            DateTime = DateTime.Now;
        }

        public string Message {
            get;
            private set;
        }

        public DateTime DateTime {
            get;
            private set;
        }
    }

    class MainVM : ViewModelBase {
        class CustomTextWriterParams : EventArgs{
            public CustomTextWriterParams(string message){
                Message = message;
            }

            public string Message {
                get;
                private set;
            }
        }

        class CustomTextWriter : TextWriter {
            public event EventHandler<CustomTextWriterParams> Callback;

            public override Encoding Encoding {
                get {
                    return Encoding.UTF8;
                }
            }

            public override void Write(string value) {
                throw new NotImplementedException();
            }

            public override void WriteLine(string value) {
                if (Callback != null) {
                    Callback(this, new CustomTextWriterParams(value));
                }
            }
        }

        static CustomTextWriter customTextWriter;
        static IOC ioc;

        static MainVM() {
            ioc = App.IOC;
            customTextWriter = new CustomTextWriter();
        }

        public MainVM() {
            customTextWriter.Callback += customTextWriter_Callback;

            LogCollection = new ObservableCollection<LogVM>();
            LogCollectionView = CollectionViewSource.GetDefaultView(LogCollection) as CollectionView;
            ScreenCollection = new ObservableCollection<ScreenVM>();

            ScreenCollection.Add(new ScreenDashboardVM());
            ScreenCollection.Add(new ScreenGameVM());

            AddLogger(ioc.Get<Logger>());
        }

        public CollectionView LogCollectionView {
            get;
            private set;
        }

        public ObservableCollection<LogVM> LogCollection {
            get;
            private set;
        }

        public ObservableCollection<ScreenVM> ScreenCollection {
            get;
            private set;
        }

        ScreenVM selectedTab;
        public ScreenVM SelectedTab {
            get {
                return selectedTab;
            }
            set {
                if (Set(ref selectedTab, value)) {
                }
            }
        }

        void customTextWriter_Callback(object sender, CustomTextWriterParams e) {
            LogCollection.Add(new LogVM(e.Message));
        }

        public void AddLogger(Logger logger) {
            var writeCollection = logger.WriterCollection;
            if (writeCollection.Contains(customTextWriter)) {
                return;
            }

            writeCollection.Add(customTextWriter);
        }

        public override void Cleanup() {
            base.Cleanup();

            customTextWriter.Callback -= customTextWriter_Callback;
        }
    }
}
