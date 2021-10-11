using System;
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
            Load();
            Read();
        }
        
        public static string GetEnvVariable(string key)
        {
            // dotenv.net doesn't load env variables on Heroku, fix that
            if (!_env.ContainsKey(key))
            {
                string value = Environment.GetEnvironmentVariable(key);

                if (value != null && value.Length > 0)
                {
                    _env[key] = value;
                }
            }

            return _env[key];
        }
    }
}