using System;
using TimHanewich.Csv;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TimHanewich.Csv
{
    public class CsvToolkit
    {

        public static CsvFile JsonToCsv(string json)
        {
            try
            {
                JArray ToConv = JArray.Parse(json);
                return JsonToCsv(ToConv);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to convert JSON string to CSV: " + ex.Message);
            }
        }

        public static CsvFile JsonToCsv(JArray array)
        {
            List<JObject> ToAdd = new List<JObject>();
            foreach (JObject jo in array)
            {
                ToAdd.Add(jo);
            }
            return JsonToCsv(ToAdd.ToArray());
        }

        public static CsvFile JsonToCsv(JObject[] ToConvert)
        {
            #region "Error checking"

            //Null?
            if (ToConvert == null)
            {
                throw new Exception("The provided ToConvert parameter was null");
            }

            //< 1?
            if (ToConvert.Length < 1)
            {
                throw new Exception("The provided array of objects was 0 in length.");
            }

            //Make sure there are no child objects
            foreach (JObject jo in ToConvert)
            {
                foreach (JProperty prop in jo.Properties())
                {
                    if (prop.Type == JTokenType.Object)
                    {
                        throw new Exception("Unable to convert array of JObject to CSV: At least one of the objects had a child object.");
                    }
                    else if (prop.Type == JTokenType.Array)
                    {
                        throw new Exception("Unable to convert array of JObject to CSV: At least one of the objects had an array of objects.");
                    }
                }
            }

            #endregion

            //Get a list of properties
            List<string> PropertyNames = new List<string>();
            foreach (JObject jo in ToConvert)
            {
                foreach (JProperty prop in jo.Properties())
                {
                    if (PropertyNames.Contains(prop.Name) == false)
                    {
                        PropertyNames.Add(prop.Name);
                    }
                }
            }

            //Begin assembling the CSV w/ headers
            CsvFile csv = new CsvFile();
            DataRow HeaderRow = csv.AddNewRow();
            foreach (string s in PropertyNames)
            {
                HeaderRow.Values.Add(s);
            }

            //Add each row
            foreach (JObject jo in ToConvert)
            {
                DataRow dr = csv.AddNewRow();
                foreach (string prop in PropertyNames)
                {
                    
                    //Add it
                    bool WasAdded = false;
                    JProperty tp = jo.Property(prop);
                    if (tp != null)
                    {
                        if (tp.Value.Type != JTokenType.Null && tp.Value.Type != JTokenType.None)
                        {
                            dr.Values.Add(tp.Value.ToString());
                            WasAdded = true;
                        }
                    }

                    //If it wasn't added, add null
                    if (WasAdded == false)
                    {
                        dr.Values.Add("");
                    }

                }
            }


            return csv;

        }
    }
}