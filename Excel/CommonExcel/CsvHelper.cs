using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonExcel
{
    public class CsvHelper
    {
        public static async Task<List<T>> ParseLine<T>(Stream stream) where T : new()
        {
            using var reader = new StreamReader(stream);

            var result = new List<T>();
            var props = typeof(T).GetProperties();
            //headers
            var headersLine = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(headersLine))
                throw new Exception("Csv parsing error: headers line is empty");
            var headers = headersLine.Split(',');
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<CsvColumnAttribute>();
                if (attr == null)
                    continue;
                if (headers[attr.Column].ToLower() != attr.Header.ToLower())
                    throw new Exception("Csv contract mismatching");
            }
            //values
            string? line;
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            {
                var columns = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");//игнорирование запятой как разделителя когда она встречается в кавычках
                var element = new T();
                foreach (var prop in props)
                {
                    var attr = prop.GetCustomAttribute<CsvColumnAttribute>();
                    if (attr == null)
                        continue;
                    prop.SetValue(element, columns[attr.Column].Replace("\"", ""));
                }
                result.Add(element);
            }
            return result;
        }
    }
}
