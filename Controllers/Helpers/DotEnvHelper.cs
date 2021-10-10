using System.Collections.Generic;
using dotenv.net;

namespace DotEnvHelpers
{
    class DotEnvHelper
    {
        private static IDictionary<string, string> _env { get; set; }
        private static bool _isLoaded { get; set; }

        static DotEnvHelper()
        {
            Load();
            Read();
        }



        private static void Read()
        {
            _env = DotEnv.Read();
        }
        
        private static void Load()
        {
            DotEnv.Load();
        }

        

        public static void RefreshEnvVariables()
        {
            _env = DotEnv.Read();
        }
        
        public static string GetEnvVariable(string key)
        {
            return _env[key];
        }
    }
}