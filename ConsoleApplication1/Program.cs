using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonString = System.IO.File.ReadAllText(@"C:\Users\t_liz\documents\visual studio 2015\Projects\ConsoleApplication2\ConsoleApplication1\TextFile1.json");
            var y = deserializeToDictionary(jsonString);
            pXMLTable x = new pXMLTable();
            var z = pXMLTable.LoadFromFile(@"C:\Users\t_liz\Documents\visual studio 2015\Projects\WindowsFormsApplication4\WindowsFormsApplication4\XMLFile1.xml");
            
            z.SaveToFile(@"C:\Users\t_liz\Documents\visual studio 2015\Projects\WindowsFormsApplication4\WindowsFormsApplication4\wtf.xml");
            var t = 1 + 1;
        }

        private static Dictionary<string, object> deserializeToDictionary(string jo)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jo);
            var values2 = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> d in values)
            {
                // if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
                if (d.Value is JObject)
                {
                    values2.Add(d.Key, deserializeToDictionary(d.Value.ToString()));
                }
                else
                {
                    values2.Add(d.Key, d.Value);
                }
            }
            return values2;
        }
    }
}
