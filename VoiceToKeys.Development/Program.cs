using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Neo.IronLua;
using VoiceToKeys.BusinessLogic;

namespace VoiceToKeys.Development {
    class Program {
        static void Main(string[] args) {
            using (var lua = new Lua()) {
                var gameLibrary = new GameLibrary();
                var gameLibraryCollection = gameLibrary.Collection;

                gameLibrary.Search();

                var game = gameLibraryCollection.FirstOrDefault();

                if (game != null) {
                    var gameLogger = game.Logger;
                    {
                        gameLogger.WriterCollection.Add(Console.Out);
                    }
                    var gameContext = new GameContext(game);
                    {
                        game.Load();
                        gameContext.LaunchAsync();
                    }
                    while (!IsKey(ConsoleKey.Escape)) ;

                    gameContext.Dispose();
                } else {
                    Console.WriteLine("No Game found");

                    while (!IsKey(ConsoleKey.Escape)) ;
                }
            }
        }

        private static bool IsKey(ConsoleKey consoleKey) {
            var consoleKeyInfo = Console.ReadKey(true);
            return consoleKey.Equals(consoleKeyInfo.Key);
        }

        
        /*
        
        */
    }
}