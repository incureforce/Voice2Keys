/* Copyright (C) 2015 Stefan Atzlesberger
 * See README.md for further informations
 */

using Neo.IronLua;
using System;
using System.Linq;
using VoiceToKeys.BusinessLogic;
using VoiceToKeys.Misc;

namespace VoiceToKeys.Development {
    class Program {
        static void Main(string[] args) {
            using (var lua = new Lua()) {
                var gameLogger = new Logger(null);
                var gameLibrary = new ProfileLibrary(gameLogger);
                var gameLibraryCollection = gameLibrary.Collection;

                gameLibrary.Search();

                var game = gameLibraryCollection.FirstOrDefault();

                if (game != null) {
                    {
                        gameLogger.WriterCollection.Add(Console.Out);
                    }
                    var gameContext = new ProfileContext(game);
                    {
                        game.Load();
                        gameContext.LaunchAsync();
                    }
                    while (!IsKey(ConsoleKey.Escape)) ;

                    gameContext.Dispose();
                }
                else {
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