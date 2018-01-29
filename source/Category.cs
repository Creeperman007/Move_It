using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Move_It
{
    class Category
    {
        public ulong id;
        public string name;

        public Category(string id, string name)
        {
            this.id = ulong.Parse(id);
            this.name = name;
        }
        public static List<Category> Categories()
        {
            List<Category> c = new List<Category>();
            JObject file = JObject.Parse(File.ReadAllText("Categories.json "));
            foreach (JObject obj in file["categories"])
            {
                c.Add(new Category(obj["id"].ToString(), obj["name"].ToString()));
            }
            return c;
        }
    }
}
