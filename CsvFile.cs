using System;
using System.Collections.Generic;
using System.Linq;

namespace TimHanewich.Csv
{
    public class CsvFile
    {
        public List<DataRow> Rows = new List<DataRow>();

        public static CsvFile CreateFromCsvFileContent(string csv_content, bool IgnoreEmptyLines = false)
        {
            CsvFile csv = new CsvFile();


            //Split up
            string splitter = Environment.NewLine;
            string[] rowss = csv_content.Split(splitter.ToArray(), StringSplitOptions.None);
            foreach (string r in rowss)
            {
                if (r != "")
                {
                    DataRow tr = DataRow.CreateFromRowData(r);
                    csv.Rows.Add(tr);
                }
            }



            return csv;
        }

        public DataRow AddNewRow()
        {
            DataRow dr = new DataRow();
            Rows.Add(dr);
            return dr;
        }

        public string GenerateAsCsvFileContent()
        {
            string ALL = "";

            foreach (DataRow dr in Rows)
            {
                string row_data = "";

                foreach (string val in dr.Values)
                {
                    string ToPost = val;

                    if (ToPost.Contains("\"") || ToPost.Contains(","))
                    {
                        ToPost = ToPost.Replace("\"", "\"\"");
                        ToPost = "\"" + ToPost + "\"";
                    }

                    row_data = row_data + ToPost + ",";
                }

                row_data = row_data.Substring(0, row_data.Length - 1);
                ALL = ALL + row_data + Environment.NewLine;
            }

            ALL = ALL.Substring(0, ALL.Length - 1);


            return ALL;
        }

    }
}