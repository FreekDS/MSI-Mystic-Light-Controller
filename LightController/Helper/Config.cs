using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MysticLightController
{
    public class Config
    {
        readonly Dictionary<string, object> _content;

        public Config(string path)
        {
            _content = new Dictionary<string, object>();
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string configPath = Path.Join(dir, path);
                string[] configEntries = File.ReadAllLines(configPath);
                foreach (string entry in configEntries)
                {
                    if (entry.TrimStart()[0] == '#')
                        continue;

                    string[] keyValue = entry.Split('=');
                    string key = keyValue[0], value = keyValue[1].Trim();

                    try
                    {
                        _content[key] = Int32.Parse(value);
                    }
                    catch (FormatException)
                    {
                        _content[key] = value;
                    }


                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Cannot read configuration file. Check if '=' is between each entry");
            }
        }

        public T Get<T>(string key)
        {
            return (T)_content[key];
        }

        public string Get(string key)
        {
            return Get<string>(key);
        }

        public string this[string key]
        {
            get { return Get(key); }
        }

    }
}
