using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToKeys.BusinessLogic {
    public class GameLibrary : IDisposable {
        static readonly DirectoryInfo DefaultDirectory = new DirectoryInfo("Profiles");

        public GameLibrary()
            : this(DefaultDirectory) {
        }

        public GameLibrary(DirectoryInfo directory) {
            if (directory == null) {
                throw new ArgumentNullException("directory");
            }

            Collection = new ObservableCollection<Game>();
            Directory = directory;
            Lua = new Lua();
        }

        public ICollection<Game> Collection {
            get;
            private set;
        }

        public DirectoryInfo Directory {
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
                var game = Game.TryNew(this, directory);
                if (game == null) {
                    return;
                }

                Collection.Add(game);
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
