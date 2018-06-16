using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Data.SqlClient;
using System.IO;

using Newtonsoft.Json;



namespace Phase2 {

    // Classes for json that timeline.js expects 

    class details {
        public string link;
        public string headline;         
    }

    class data {
        public string date;
        public details details;

        public data(string date, string link, string headline) {
            this.date = date;

            details = new details();
            details.link = link;
            details.headline = headline;
        }
    }

    //class Topic {
    class mediaOutlet {
        public string name;
        public List<data> data;

        public mediaOutlet(string name) {
            this.name = name;
            data = new List<data>();
        }
    }

    class conflict {
        public string name;
        public string description;
        public string id;
        public List<mediaOutlet> mediaOutlets;
        public conflict(string name, string description, string id) {
            this.name = name;
            this.description = description;
            this.id = id;
            mediaOutlets = new List<mediaOutlet>();
        }
    }




    public class TimelineImport {
        public static int MakeJson(string outputFileName) {
            
            string connectionString = "Server=SCOTT-PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";

            var mediaOutlets = new List<mediaOutlet>();

            var conflicts = new List<conflict>();
            int stories = 0;
            string query = "SELECT * FROM ConflictTimelineView ORDER BY Conflict, MediaOutlet";
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    conflict currentConflict = null;
                    mediaOutlet currentMediaOutlet = null;
                    while (reader.Read()) {

                        var conflictName = reader["Conflict"].ToString();
                        if (currentConflict == null || (conflictName != currentConflict.name)) {
                            var oldConflict = currentConflict;

                            currentConflict = new conflict(conflictName, reader["ConflictDescription"].ToString(), reader["ID"].ToString());
                            conflicts.Add(currentConflict);
                            currentMediaOutlet = null;

                            if (oldConflict != null && oldConflict.mediaOutlets.Count != 0)
                                WriteConflictToJson(outputFileName + oldConflict.id + ".js", oldConflict);

                        }

                        var mediaOutletName = reader["MediaOutlet"].ToString();
                        if (currentMediaOutlet == null || (mediaOutletName != currentMediaOutlet.name)) {
                            currentMediaOutlet = new mediaOutlet(mediaOutletName);
                            currentConflict.mediaOutlets.Add(currentMediaOutlet);
                        }

                        currentMediaOutlet.data.Add(Makedata(reader));
                        stories++;
                    }
                }
            }
            //WriteTopicsToJson(outputFileName, conflicts);
            return stories;
        }

        private static data Makedata(SqlDataReader reader) {
            return new data(
                reader["date"].ToString()
                , reader["link"].ToString()
                , reader["headline"].ToString()
            );
        }

        private static void WriteConflictToJson(string outputFileName, conflict conflict) {
            string json = JsonConvert.SerializeObject(conflict);
            var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();
            System.IO.File.WriteAllText(outputFileName, niceJson);
        }
    }
}
