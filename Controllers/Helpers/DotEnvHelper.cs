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
            System.Console.WriteLine("\n\n\nLOADING ENV VARIABLES\n\n\n");
            DotEnv.Load();
        }

        

        public static void RefreshEnvVariables()
        {
            Load();
            Read();
        }
        
        public static string GetEnvVariable(string key)
        {
            RefreshEnvVariables();
            System.Console.WriteLine("\n\n\n_env: " + _env);
            return _env[key];
        }
    }
}