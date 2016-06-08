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
            var o = JObject.Parse(jsonString);
            var z = transformToPXmlTable(o);
            
            //var z = pXMLTable.LoadFromFile(@"C:\Users\t_liz\Documents\visual studio 2015\Projects\WindowsFormsApplication4\WindowsFormsApplication4\XMLFile1.xml");
            
            z.SaveToFile(@"C:\Users\t_liz\Documents\visual studio 2015\Projects\WindowsFormsApplication4\WindowsFormsApplication4\reconstructed.xml");
            var t = 1 + 1;
        }


        private static pXMLTable transformToPXmlTable(JObject root)
        {
            pXMLTable p = new pXMLTable();

            // pXMLTable attributes
            p.desc      = "Part Table";
            p.fixColumn = "C1";
            p.fileMigrate = "False"; // may not exist for every xml
            p.version   = 1.0M;

            // ColumnConstView
            p.ColumnConstView           = new pXMLTableColumnConstView();
            p.ColumnConstView.desc      = "Parameter-driven Display";
            p.ColumnConstView.id        = "CCV1";
            p.ColumnConstView.viewKey   = "3d";
            p.ColumnConstView.viewName  = "AecbPartRecipe";
            
            p.ColumnConstView.Images = new pXMLTableColumnConstViewImages();
            p.ColumnConstView.Images.Image = new pXMLTableColumnConstViewImagesImage();
            p.ColumnConstView.Images.Image.URL = new pXMLTableColumnConstViewImagesImageURL();
            p.ColumnConstView.Images.Image.URL.title = "Part Reference Image";
            p.ColumnConstView.Images.Image.URL.href = root.SelectToken("additionalInfo").SelectToken("bmpPath").ToString();

            p.ColumnConstView.Recipe = root.SelectToken("additionalInfo").SelectToken("dwgPath").ToString();

            // ColumnUnique
            p.ColumnUnique = new pXMLTableColumnUnique();
            p.ColumnUnique.desc = "Primary Key";
            p.ColumnUnique.datatype = "string";
            p.ColumnUnique.name = "UUID";
            p.ColumnUnique.visible = 0;
            
            p.ColumnUnique.RowUnique = new List<pXMLTableColumnUniqueRowUnique>();
            JArray columnUniques = (JArray)root.SelectToken("additionalInfo").SelectToken("ColumnUnique");
            for (int i = 0; i < columnUniques.Count; i++)
            {
                var ru = new pXMLTableColumnUniqueRowUnique();
                ru.Value = columnUniques[i].ToString();
                ru.id = "r" + i;
                p.ColumnUnique.RowUnique.Add(ru);
            }

            // Column
            p.Column = new List<pXMLTableColumn>();
            IEnumerable<KeyValuePair<string, JToken>> columns = (JObject)root.SelectToken("basicTable");    // downcast from JToken to JObject
            foreach (KeyValuePair<string, JToken> kvp in columns)
            {
                var column = new pXMLTableColumn();
                // "decompress" attribute with assumed fixed order
                string[] attrSeq = ((JArray) kvp.Value.SelectToken("attr")).ToObject<string[]>();
                column.dataType = attrSeq[0];
                column.unit     = attrSeq[1];
                column.name     = attrSeq[2];
                column.id       = attrSeq[3];
                column.visible  = Convert.ToByte(Int32.Parse(attrSeq[4]));
                column.context  = attrSeq[5];
                column.index    = Convert.ToByte(Int32.Parse(attrSeq[6]));
                column.desc     = kvp.Key;                                      // remember in xml-to-json transformation, columns are keyed by "desc"

                // reconstruct rows
                string[] entries = ((JArray) kvp.Value.SelectToken("entries")).ToObject<string[]>();
                for (int i = 0; i < entries.Length; i++)
                {
                    var row = new pXMLTableColumnRow();
                    row.id = "r" + i;
                    row.Value = Convert.ToDecimal(entries[i]);
                    column.Row.Add(row);
                }

                p.Column.Add(column);
            }


            // ColumnConstList
            p.ColumnConstList = new List<pXMLTableColumnConstList>();
            IEnumerable<KeyValuePair<string, JToken>> columnConstLists = (JObject)root.SelectToken("constantLists");
            foreach (KeyValuePair<string, JToken> kvp in columnConstLists)
            {
                var columnConstList = new pXMLTableColumnConstList();
                // "decompress" attribute
                string[] attrSeq = ((JArray)kvp.Value.SelectToken("attr")).ToObject<string[]>();
                columnConstList.dataType = attrSeq[0];
                columnConstList.unit = attrSeq[1];
                columnConstList.name = attrSeq[2];
                columnConstList.id = attrSeq[3];
                columnConstList.visible = Convert.ToByte(Int32.Parse(attrSeq[4]));
                columnConstList.context = attrSeq[5];
                columnConstList.index = Convert.ToByte(Int32.Parse(attrSeq[6]));
                columnConstList.desc = kvp.Key;

                // reconstruct items
                string[] entries = ((JArray)kvp.Value.SelectToken("entries")).ToObject<string[]>();
                for (int i = 0; i < entries.Length; i++)
                {
                    var item = new pXMLTableColumnConstListItem();
                    item.id = "i" + i;
                    item.Value = entries[i];
                    columnConstList.Item.Add(item);
                }

                p.ColumnConstList.Add(columnConstList);
            }


            // ColumnConst
            p.ColumnConst = new List<pXMLTableColumnConst>();
            IEnumerable<KeyValuePair<string, JToken>> columnConsts = (JObject)root.SelectToken("constants");
            foreach (KeyValuePair<string, JToken> kvp in columnConsts)
            {
                var columnConst = new pXMLTableColumnConst();
                // "decompress" attribute
                string[] attrSeq    = ((JArray)kvp.Value.SelectToken("attr")).ToObject<string[]>();
                columnConst.dataType= attrSeq[0];
                columnConst.unit    = attrSeq[1];
                columnConst.name    = attrSeq[2];
                columnConst.id      = attrSeq[3];
                columnConst.visible = Convert.ToByte(Int32.Parse(attrSeq[4]));
                columnConst.context = attrSeq[5];
                columnConst.index   = Convert.ToByte(Int32.Parse(attrSeq[6]));
                columnConst.desc    = kvp.Key;

                // reconstruct value
                string[] entries    = ((JArray)kvp.Value.SelectToken("entries")).ToObject<string[]>();
                columnConst.Value   = entries[0];
                p.ColumnConst.Add(columnConst);
            }
            
            // ColumnCalc
            p.ColumnCalc = new List<pXMLTableColumnCalc>();
            IEnumerable<KeyValuePair<string, JToken>> columnCalcs = (JObject)root.SelectToken("calculations");
            foreach (KeyValuePair<string, JToken> kvp in columnCalcs)
            {
                var columnCalc = new pXMLTableColumnCalc();
                // "decompress" attribute
                string[] attrSeq = ((JArray)kvp.Value.SelectToken("attr")).ToObject<string[]>();
                columnCalc.dataType = attrSeq[0];
                columnCalc.unit = attrSeq[1];
                columnCalc.name = attrSeq[2];
                columnCalc.id = attrSeq[3];
                columnCalc.visible = Convert.ToByte(Int32.Parse(attrSeq[4]));
                columnCalc.context = attrSeq[5];
                columnCalc.index = Convert.ToByte(Int32.Parse(attrSeq[6]));
                columnCalc.desc = kvp.Key;

                // reconstruct value
                string[] entries = ((JArray)kvp.Value.SelectToken("entries")).ToObject<string[]>();
                columnCalc.Value = entries[0];
                p.ColumnCalc.Add(columnCalc);
            }

            return p;
        }
    }
}
