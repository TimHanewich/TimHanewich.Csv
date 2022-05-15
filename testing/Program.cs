using System;
using TimHanewich.Csv;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace testing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CsvFile csv = CsvFile.CreateFromCsvFileContent(System.IO.File.ReadAllText(@"C:\Users\tahan\Downloads\example.csv"), false);
            Console.WriteLine(JsonConvert.SerializeObject(csv));
            JArray ja = csv.ToJson();
            Console.WriteLine(ja.ToString());
        }
    }
}