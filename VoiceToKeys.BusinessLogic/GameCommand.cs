using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToKeys.BusinessLogic {
    static public class GameCommandEnumerable {
        static public IEnumerable<GameCommand> CommandsBy(this IEnumerable<GameCommand> commandCollection, GameCommand commandParent) {
            var commands =
                from command in commandCollection
                where command.Parent == commandParent
                select command;
            return commands.ToArray();
        }
    }

    public class GameCommand {
        ICollection<GameCommand> collection;

        internal GameCommand(ICollection<GameCommand> commandCollection) {
            collection = commandCollection;
            collection.Add(this);
        }

        public GameCommand Parent {
            get;
            internal set;
        }

        public string Text {
            get;
            internal set;
        }

        public string Full {
            get;
            internal set;
        }

        public Func<object, LuaResult> FunctionHandler {
            get;
            internal set;
        }

        public IEnumerable<GameCommand> Commands {
            get {
                return collection.CommandsBy(this);
            }
        }

        internal void Call() {
            if (FunctionHandler != null) {
                FunctionHandler(this);
            }
        }

        public override string ToString() {
            return string.Format("{{{0} Text: '{1}', Full: '{2}', Collection: {3}}}", GetType().Name, Text, Full, Commands);
        }
    }
}
