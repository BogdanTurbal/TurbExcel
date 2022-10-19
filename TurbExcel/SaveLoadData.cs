using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TurbExcel
{
    public class SaveLoadData
    {
        public class TupleConverter<T1, T2> : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var key = Convert.ToString(value).Trim('(').Trim(')');
                var parts = Regex.Split(key, (", "));
                var item1 = (T1)TypeDescriptor.GetConverter(typeof(T1)).ConvertFromInvariantString(parts[0])!;
                var item2 = (T2)TypeDescriptor.GetConverter(typeof(T2)).ConvertFromInvariantString(parts[1])!;
                return new ValueTuple<T1, T2>(item1, item2);
            }
        }
        private static string _path = "";
        public static void Serialize(Dictionary<string, Cell> dictionary, int cl, int r, string path)
        {
            Dictionary<(string, string), string> newDict = new Dictionary<(string, string), string>();
            newDict[("rows", "")] = r.ToString();
            newDict[("columns", "")] = cl.ToString();
            _path = path;
            foreach ((string s, Cell c) in dictionary)
            {
                newDict[(c.Col.ToString(), c.Row.ToString())] = c.Exp;
            }
            try // try to serialize the collection to a file
            {
                
                File.WriteAllText(path, JsonConvert.SerializeObject(newDict));
                /*using (stream)
                {
                    // create BinaryFormatter
                    BinaryFormatter bin = new BinaryFormatter();
                    // serialize the collection (EmployeeList1) to file (stream)
                    bin.Serialize(stream, dictionary);
                }*/
            }
            catch (Exception)
            {
            }
        }
        
        public static Dictionary<(string, string), string> Deserialize(string path)
        {
            Dictionary<(string, string), string>? ret = new Dictionary<(string, string), string>();
            TypeDescriptor.AddAttributes(typeof((string, string)), new TypeConverterAttribute(typeof(TupleConverter<string, string>)));
            ret = JsonConvert.DeserializeObject<Dictionary<(string, string), string>>(File.ReadAllText(path));
            return ret;
        }
    }
}
