/* Copyright (C) 2015 Stefan Atzlesberger
 * See README.md for further informations
 */

using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VoiceToKeys.Misc;

namespace VoiceToKeys.BusinessLogic {
    public class ProfileLibrary : IDisposable {
        static readonly DirectoryInfo DefaultDirectory = new DirectoryInfo("Profiles");

        public ProfileLibrary(Logger logger)
            : this(DefaultDirectory, logger) {
        }

        public ProfileLibrary(DirectoryInfo directory, Logger logger) {
            if (directory == null) {
                throw new ArgumentNullException("directory");
            }

            if (logger == null) {
                throw new ArgumentNullException("logger");
            }

            Collection = new ObservableCollection<Profile>();
            Directory = directory;
            Logger = logger;
            Lua = new Lua();
        }

        public ICollection<Profile> Collection {
            get;
            private set;
        }

        public DirectoryInfo Directory {
            get;
            private set;
        }

        public Logger Logger {
            get;
            private set;
        }

        public Lua Lua {
            get;
            private set;
        }

        public void Search() {
            var directories =
                from directory in Directory.GetDirectories()
                where Collection.SingleOrDefault(p => p.Directory == directory) == null
                select directory;

            foreach (var directory in directories) {
                var profile = Profile.TryNew(this, directory);
                if (profile == null) {
                    return;
                }

                Collection.Add(profile);
            }
        }

        public void Dispose() {
            if (Lua != null) {
                Lua.Dispose();
                Lua = null;
            }
        }
    }
}
