using System;
using TimHanewich.Csv;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TimHanewich.Csv
{
    public static class CsvToolkit
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
    
        public static JArray ToJson(this CsvFile csv)
        {
            //Get the property names from the first row
            DataRow HeaderRow = csv.Rows[0];
            List<string> PropertyNames = new List<string>();
            foreach (string val in HeaderRow.Values)
            {
                if (PropertyNames.Contains(val))
                {
                    throw new Exception("Unable to convert CSV to JSON: There was a duplicate property name in the header row.");
                }
                PropertyNames.Add(val);
            }

            //Construct
            JArray ToReturn = new JArray();
            for (int t = 1; t < csv.Rows.Count; t++)
            {
                JObject jo = new JObject();
                for (int cn = 0; cn < PropertyNames.Count; cn++)
                {
                    bool added = false;

                    //Null?
                    if (added == false)
                    {
                        string val = csv.Rows[t].Values[cn];
                        if (val == "")
                        {
                            jo.Add(PropertyNames[cn], null);
                            added = true;
                        }
                    }

                    //Int?
                    if (added == false)
                    {
                        try
                        {
                            jo.Add(PropertyNames[cn], Convert.ToInt32(csv.Rows[t].Values[cn]));
                            added = true;
                        }
                        catch
                        {

                        }
                    }
                    

                    //Float?
                    if (added == false)
                    {
                        try
                        {
                            jo.Add(PropertyNames[cn], Convert.ToSingle(csv.Rows[t].Values[cn]));
                            added = true;
                        }
                        catch
                        {

                        }
                    }

                    //boolean?
                    if (added == false)
                    {
                        try
                        {
                            jo.Add(PropertyNames[cn], Convert.ToBoolean(csv.Rows[t].Values[cn]));
                            added = true;
                        }
                        catch
                        {

                        }
                    }
                    
                    //If it hasn't been added as a value, add it as a string
                    if (added == false)
                    {
                        jo.Add(PropertyNames[cn], csv.Rows[t].Values[cn]);
                    }
                }
                ToReturn.Add(jo);
            }

            return ToReturn;
        }
    }
}