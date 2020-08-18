using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace marlinX
{
    static class Db
    {
        private static Dictionary<string,int> cache = new Dictionary<string, int>();
        private  static readonly object lockObject = new object();
        static Db()
        {
            cache = getFromDatabase();
        }

        public static void SaveToDatabase(IEnumerable<string> words)
        {
            lock (lockObject)
            {
                foreach (var word in words)
                {
                    if (!cache.ContainsKey(word.ToLower()))
                        cache[word.ToLower()] = 0;
                    cache[word.ToLower()] += 1;
                }

                File.WriteAllText("db.txt", JsonConvert.SerializeObject(cache));
            }
        }

        public static Dictionary<string, int> getFromDatabase(int count = 150)
        {
            try
            {
                lock (lockObject)
                {
                    string text = File.ReadAllText("db.txt");
                    Dictionary<string, int> result = JsonConvert.DeserializeObject<Dictionary<string, int>>(text);
                    return result.OrderByDescending(x=>x.Value).Take(count).ToDictionary(data=> data.Key,data=>data.Value);
                }
            }
            catch (FileNotFoundException fileNotFound)
            {
                return new Dictionary<string, int>();
            }
        }

    }
}
