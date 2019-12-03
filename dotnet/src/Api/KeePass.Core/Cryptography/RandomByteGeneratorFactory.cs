using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Cryptography
{
    public static class RandomByteGeneratorFactory
    {
        private static List<Type> s_engines;


        static RandomByteGeneratorFactory()
        {
            s_engines = new List<Type>();
            s_engines.Add(null);
            s_engines.Add(null);
            s_engines.Add(typeof(Salsa20RandomByteGenerator));
        }
        

        public static IRandomByteGeneratorEngine GetGenerator(int id)
        {
            var type = s_engines[id];
            return (IRandomByteGeneratorEngine)Activator.CreateInstance(type);
        }
    }
}
