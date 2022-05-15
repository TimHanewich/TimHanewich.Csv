using System;
using System.Collections.Generic;

namespace TimHanewich.Csv
{
    public class DataRow
    {
        public List<string> Values = new List<string>();
        

        public static DataRow CreateFromRowData(string row_data)
        {
            DataRow dr = new DataRow();


            //Get comma separator locations
            List<int> CommaSeperatorLocations = new List<int>();
            int t = 0;
            bool inside_quote = false;
            for (t = 0; t <= (row_data.Length - 1); t++)
            {
                string this_char = row_data.Substring(t, 1);

                if (this_char == ",")
                {
                    if (inside_quote == false)
                    {
                        CommaSeperatorLocations.Add(t);
                    }
                }
                else if (this_char == "\"")
                {
                    if (inside_quote)
                    {
                        inside_quote = false;
                    }
                    else
                    {
                        inside_quote = true;
                    }
                }
            }



            //Get strings
            int drop_location = 0;
            if (CommaSeperatorLocations.Count > 0)
            {
                foreach (int commaloc in CommaSeperatorLocations)
                {
                    string this_val = row_data.Substring(drop_location, commaloc - drop_location);

                    //Remove the quotes on the end if they exist.
                    if (this_val.Length > 0)
                    {
                        if (this_val.Substring(0, 1) == "\"")
                        {
                            if (this_val.Substring(this_val.Length - 1, 1) == "\"")
                            {
                                this_val = this_val.Substring(1, this_val.Length - 2);
                            }
                        }
                    }


                    //Remove double quotations.
                    this_val = this_val.Replace("\"\"", "\"");



                    dr.Values.Add(this_val);
                    drop_location = commaloc + 1;
                }
                string final_val = row_data.Substring(drop_location);
                final_val = final_val.Replace("\"\"", "");
                dr.Values.Add(final_val);
            }
            else
            {
                dr.Values.Add(row_data);
            }


            return dr;
        }
    }
}