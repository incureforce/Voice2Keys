﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToKeys.VMs {
    class ScreenVM : ViewModelBase {
        protected ScreenVM(string name) {
            Name = name;
        }

        public string Name {
            get;
            private set;
        }
    }
}
