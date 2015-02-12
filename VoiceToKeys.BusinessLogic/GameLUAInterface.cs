using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceToKeys.Misc;

namespace VoiceToKeys.BusinessLogic {
    public class GameLUAInterface {
        class API {
            private static void SendInput(VirtualKey virtualKey, KeyboardInputEventFlags flags) {
                var inputs = new Input[] {
                    new Input {
                        Type = InputType.KEYBOARD,
                        InputUnion = new InputUnion {
                            Keyboard = new KeyboardInput {
                                ScanCode = (ushort)Win32.MapVirtualKey((ushort)virtualKey, MapVirtualKeyType.VirtualKeyToScanCode),
                                VirtualKey = virtualKey,
                                EventFlags = flags | KeyboardInputEventFlags.SCANCODE,
                            },
                        },
                    },
                };
                Win32.SendInput((uint)inputs.Length, inputs, Input.Size);
            }

            internal Logger Logger {
                get;
                set;
            }

            internal ICollection<string> GameProcessNameCollection {
                get;
                set;
            }
            internal ICollection<GameCommand> GameCommandCollection {
                get;
                set;
            }

            public void Log(string format, params object[] args) {
                Logger.Log(format, args);
            }

            public void AddProcessName(string processName) {
                GameProcessNameCollection.Add(processName);
            }

            public void KeyDown(VirtualKey virtualKey) {
                SendInput(virtualKey, 0);
            }

            public void KeyUp(VirtualKey virtualKey) {
                SendInput(virtualKey, KeyboardInputEventFlags.KEYUP);
            }

            public void Wait(int milliseconds) {
                Thread.Sleep(milliseconds);
            }

            public GameCommand AddCommand(string commandText, GameCommand commandParent, Func<object, LuaResult> commandFunctionHandler) {
                var commandFull = commandText;
                if (commandParent != null) {
                    commandFull = string.Concat(commandParent.Full, " ", commandText);
                }

                return new GameCommand(GameCommandCollection) {
                    Text = commandText,
                    Full = commandFull,
                    Parent = commandParent,
                    FunctionHandler = commandFunctionHandler,
                };
            }
        }

        static LuaTable EnumTable(Type type) {
            var luaTable = new LuaTable();
            foreach (var enumKey in Enum.GetValues(type)) {
                luaTable.SetMemberValue(string.Empty + enumKey, enumKey);
            }
            return luaTable;
        }

        API api;
        LuaGlobal apiEnvironment;

        public GameLUAInterface(Lua lua, Game game) {
            Lua = lua;
            Game = game;

            apiEnvironment = new LuaGlobal(lua);
            api = new API() {
                Logger = game.Logger,
                GameCommandCollection = game.CommandCollection,
                GameProcessNameCollection = game.ProcessNameCollection,
            };
        }

        public Game Game {
            get;
            private set;
        }

        public Lua Lua {
            get;
            private set;
        }

        public LuaChunk LuaChunk {
            get;
            private set;
        }

        public void ResetExecute(StreamReader streamReader) {
            apiEnvironment.Clear();
            apiEnvironment.SetMemberValue("api", api);
            apiEnvironment.SetMemberValue("virtualKey", EnumTable(typeof(VirtualKey)));

            LuaChunk = Lua.CompileChunk(streamReader, Game.Name, null);

            apiEnvironment.DoChunk(LuaChunk);
        }
    }
}
