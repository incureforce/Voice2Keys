using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToKeys.Misc {
    public class IOC {
        static public readonly IOC Default = new IOC();

        Dictionary<Type, Delegate> typeDictionary = new Dictionary<Type,Delegate>();

        public void Set<TArg>(Func<TArg> factory) {
            typeDictionary[typeof(TArg)] = factory;
        }

        public TArg Get<TArg>() {
            var factory = typeDictionary[typeof(TArg)] as Func<TArg>;
            return factory();
        }
    }
}
