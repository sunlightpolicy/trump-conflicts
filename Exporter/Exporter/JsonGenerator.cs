using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.IO;

using Newtonsoft.Json;



namespace Phase2 {

    public class EthicsDocumentForJson {
        public string name { get; set; }
        public string date { get; set; }
        public string link { get; set; }

        public EthicsDocumentForJson (string name, string date, string link) {
            this.name = name;
            this.date = date;
            this.link = link;
        }
    }

    public class FamilyMemberBusinessWithEthics {
        public string familyMember { get; set; }
        public string description { get; set; }
        public string business { get; set; }
        public string conflictStatus { get; set; }
        public List<EthicsDocumentForJson> ethicsDocuments;

        public FamilyMemberBusinessWithEthics(string familyMember, string description, string business, string conflictStatus) {
            this.familyMember = familyMember;
            this.description = description;
            this.business = business;
            this.conflictStatus = conflictStatus;

            ethicsDocuments = new List<EthicsDocumentForJson>();
        }
    }

    class FamilyMemberBusinessEthicsForConflict {
        public string conflictId { get; set; }
        public string conflict { get; set; }
        public string conflictDescription { get; set; }
        public List<FamilyMemberBusinessWithEthics> familyMemberBusinessWithEthicsList;

        public FamilyMemberBusinessEthicsForConflict(string conflictId, string conflict, string conflictDescription) {
            this.conflictId = conflictId;
            this.conflict = conflict;
            this.conflictDescription = conflictDescription;
            familyMemberBusinessWithEthicsList = new List<FamilyMemberBusinessWithEthics>();
        }
    }



    public class JsonGenerator {

        public static void Run(string path) {
            var stories = MakeStories();
            WriteStoryJson(path, stories);

            List<FamilyMemberBusinessEthicsForConflict> ethicsForConflict = MakeEthicsInfos();
            WriteEthicsInfosToJson(path, ethicsForConflict);
        }

        private static void WriteEthicsInfosToJson(string path, List<FamilyMemberBusinessEthicsForConflict> ethicsForConflicts) {
            string fullPath = path + "ethics\\";
            try {
                Directory.Delete(fullPath, true);
            } catch (Exception e) {
            }
            Directory.CreateDirectory(fullPath);

            foreach (FamilyMemberBusinessEthicsForConflict conflict in ethicsForConflicts) {
                string json = JsonConvert.SerializeObject(conflict);
                var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();

                System.IO.File.WriteAllText(fullPath + conflict.conflictId + ".json", niceJson);
            }
        }

        private static List<Conflicts.Story> MakeStories() {
            var stories = new List<Conflicts.Story>();

            string connString = "Server=PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";
            string query = "SELECT * FROM StoryConflictView";
            using (SqlConnection conn = new SqlConnection(connString)) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                        AddStory(reader, stories);

                    Console.WriteLine(stories.Count.ToString() + " Stories");
                }
            }
            return stories;
        }

        private static void AddStory(SqlDataReader reader, List<Conflicts.Story> stories) {

            var story = new Conflicts.Story(
                reader["ConflictDescription"].ToString() // Description
                , reader["FamilyMember"].ToString() // FamilyMember
                , reader["Conflict"].ToString() // ConflictingEntity // NOW Conflict
                , reader["ConflictStatus"].ToString() // Category
                , reader["ConflictNotes"].ToString() // Notes
                , reader["ConflictUpdateDate"].ToString() // DateChanged
                , reader["MediaOutlet"].ToString() // Source - NOW MediaOutlet
                , reader["Link"].ToString() // Link
                , reader["Date"].ToString() // Date
                , reader["Headline"].ToString() // Headline

                , reader["ConflictId"].ToString()
                , reader["EthicsCount"].ToString() != "0"
            );
            stories.Add(story);
        }

        private static void WriteStoryJson(string path, List<Conflicts.Story> stories) {
            string json = JsonConvert.SerializeObject(stories);
            var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();
            System.IO.File.WriteAllText(path + "stories3.json", niceJson);
        }

        private static List<FamilyMemberBusinessEthicsForConflict> MakeEthicsInfos() {

            var ethics = new List<FamilyMemberBusinessEthicsForConflict>();

            string connString = "Server=PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";
            string query = "SELECT * FROM BusinessConflictView ORDER BY ConflictID, FamilyMember, Business, EthicsDocument";
            using (SqlConnection conn = new SqlConnection(connString)) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                        AddEthics(reader, ethics);
                }
            }
            return ethics;
        }

        private static void AddEthics(SqlDataReader reader, List<FamilyMemberBusinessEthicsForConflict> ethicsInfos) {

            var conflictId = reader["ConflictId"].ToString();
            var business = reader["Business"].ToString();

            FamilyMemberBusinessEthicsForConflict lastConflict = (ethicsInfos.Count == 0) ? null : ethicsInfos.Last();
            bool addConflict = ((lastConflict == null) || (lastConflict.conflictId != conflictId));
            if (addConflict) {
                AddConflict(reader, ethicsInfos);
                lastConflict = ethicsInfos.Last();
            }

            FamilyMemberBusinessWithEthics lastBusiness = null;
            if (lastConflict != null)
                lastBusiness = (ethicsInfos.Last().familyMemberBusinessWithEthicsList.Count == 0) ? null : ethicsInfos.Last().familyMemberBusinessWithEthicsList.Last();
            bool addBusiness = ((lastBusiness == null) || (lastConflict.familyMemberBusinessWithEthicsList.Last().business != lastBusiness.business));
            if (addBusiness) {
                AddBusiness(reader, ethicsInfos.Last());
                //ethicsInfos.Last().FamilyMemberBusinessWithEthicsList.Last();
                lastBusiness = ethicsInfos.Last().familyMemberBusinessWithEthicsList.Last();
            }

            lastBusiness.ethicsDocuments.Add(new EthicsDocumentForJson(
                reader["EthicsDocument"].ToString()
                , reader["EthicsDocumentDate"].ToString()
                , reader["EthicsDocumentLink"].ToString()
                )); 
        }

        private static void AddConflict(SqlDataReader reader, List<FamilyMemberBusinessEthicsForConflict> ethicsInfos) {
            ethicsInfos.Add(new FamilyMemberBusinessEthicsForConflict(
                reader["ConflictID"].ToString()
                , reader["Conflict"].ToString()
                , reader["ConflictDescription"].ToString()));
        }

        private static void AddBusiness(SqlDataReader reader, FamilyMemberBusinessEthicsForConflict conflict) {
            conflict.familyMemberBusinessWithEthicsList.Add(new FamilyMemberBusinessWithEthics(
                reader["FamilyMember"].ToString() 
                , reader["Description"].ToString() 
                , reader["Business"].ToString() 
                , reader["ConflictStatus"].ToString() 
            ));
        }
    }
}

