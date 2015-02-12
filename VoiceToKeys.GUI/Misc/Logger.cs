using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceToKeys.Misc {
    abstract class LoggerModule : IFormattable {
        const string RegexPattern = "{$name(:[^}]+)?}";
        public LoggerModule(string name) {
            var st = RegexPattern.Replace("$name", name);
            Regex = new Regex(st);
        }

        public Regex Regex {
            get;
            private set;
        }

        public abstract string ToString(string format, IFormatProvider formatProvider);
    }

    class SimpleLoggerModule : LoggerModule {
        public SimpleLoggerModule(string name, Func<string, IFormatProvider, string> handler)
            : base(name) {
            Handler = handler;
        }

        private Func<string, IFormatProvider, string> Handler {
            get;
            set;
        }

        public override string ToString(string format, IFormatProvider formatProvider) {
            return Handler(format, formatProvider);
        }
    }

    class Logger {
        static public readonly Logger Default = new Logger("Default");
        static public readonly LoggerModule
            CurrentMethodModule = new SimpleLoggerModule("cm", CurrentMethodModuleHandler),
            CurrentDateTimeModule = new SimpleLoggerModule("ct", CurrentDateTimeModuleHandler);

        private static string CurrentDateTimeModuleHandler(string arg1, IFormatProvider arg2) {
            var dateTime = DateTime.Now;

            return dateTime.ToString(arg1);
        }

        private static string CurrentMethodModuleHandler(string arg1, IFormatProvider arg2) {
            var stackFrame = new StackFrame(7);
            var method = stackFrame.GetMethod();
            var type = method.DeclaringType;

            return string.Concat(type.Name, (method.IsStatic ? "::" : "->"), method.Name);
        }

        public Logger(object context) {
            Prefix = string.Empty;
            Context = context;
            Modules = new Collection<LoggerModule>();
            WriterCollection = new Collection<TextWriter>();
        }

        public string Prefix {
            get;
            private set;
        }

        public ICollection<LoggerModule> Modules {
            get;
            private set;
        }

        public ICollection<TextWriter> WriterCollection {
            get;
            private set;
        }

        public object Context {
            get;
            private set;
        }

        public void Log(string format, params object[] args) {
            var moduleRegex = default(Regex);
            var moduleRegexGroups = default(GroupCollection);
            var thread = Thread.CurrentThread;
            var threadCulture = thread.CurrentCulture;

            format = string.Concat(Prefix, format);

            foreach (var module in Modules) {
                moduleRegex = module.Regex;

                format = moduleRegex.Replace(format, (p) => {
                    moduleRegexGroups = p.Groups;
                    return module.ToString(moduleRegexGroups[1].ToString(), threadCulture);
                });
            }

            foreach (var writer in WriterCollection) {
                writer.WriteLine(string.Format(format, args));
            }
        }
    }
}
