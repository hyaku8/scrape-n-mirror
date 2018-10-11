using scrapenmirror.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace scrapenmirror.Models
{
    public class Scrape : IRepositoryItem<string>
    {
        public string Id { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        public static string GenerateId(int seed, int length = 5)
        {
            Random r = new Random(seed);
            string id = "";
            for (int i = 0; i < length; i++)
            {
                id += characters[r.Next(0, characters.Length - 1)];
            }
            return id;
        }
    }
}
