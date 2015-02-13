using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace VoiceToKeys.BusinessLogic {
    static public class GameCommandEnumerable {
        static public IEnumerable<ProfileCommand> CommandsBy(this IEnumerable<ProfileCommand> commandCollection, ProfileCommand commandParent) {
            var commands =
                from command in commandCollection
                where command.Parent == commandParent
                select command;
            return commands.ToArray();
        }
    }

    public class ProfileCommand {
        ICollection<ProfileCommand> collection;

        internal ProfileCommand(ICollection<ProfileCommand> commandCollection) {
            collection = commandCollection;
            collection.Add(this);
        }

        [PrettyObject]
        public ProfileCommand Parent {
            get;
            internal set;
        }

        [PrettyObject]
        public string Text {
            get;
            internal set;
        }

        public string Full {
            get;
            internal set;
        }

        [PrettyObject]
        public Func<object, LuaResult> FunctionHandler {
            get;
            internal set;
        }

        [PrettyObject]
        public IEnumerable<ProfileCommand> Commands {
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
            return this.ToPrettyString();
        }
    }
}
