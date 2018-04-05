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
        public string Name { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }

        public EthicsDocumentForJson (string name, string date, string link) {
            Name = name;
            Date = date;
            Link = link;
        }
    }

    public class FamilyMemberBusinessWithEthics {
        public string FamilyMember { get; set; }
        public string Description { get; set; }
        public string Business { get; set; }
        public string ConflictStatus { get; set; }
        public List<EthicsDocumentForJson> EthicsDocuments;

        public FamilyMemberBusinessWithEthics(string familyMember, string description, string business, string conflictStatus) {
            FamilyMember = familyMember;
            Description = description;
            Business = business;
            ConflictStatus = conflictStatus;

            EthicsDocuments = new List<EthicsDocumentForJson>();
        }
    }

    class FamilyMemberBusinessEthicsForConflict {
        public string ConflictId { get; set; }
        public List<FamilyMemberBusinessWithEthics> FamilyMemberBusinessWithEthicsList;

        public FamilyMemberBusinessEthicsForConflict(string conflictId) {
            ConflictId = conflictId;
            FamilyMemberBusinessWithEthicsList = new List<FamilyMemberBusinessWithEthics>();
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

                System.IO.File.WriteAllText(fullPath + conflict.ConflictId + ".json", niceJson);
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
            bool addConflict = ((lastConflict == null) || (lastConflict.ConflictId != conflictId));
            if (addConflict) {
                AddConflict(reader, ethicsInfos);
                lastConflict = ethicsInfos.Last();
            }

            FamilyMemberBusinessWithEthics lastBusiness = null;
            if (lastConflict != null)
                lastBusiness = (ethicsInfos.Last().FamilyMemberBusinessWithEthicsList.Count == 0) ? null : ethicsInfos.Last().FamilyMemberBusinessWithEthicsList.Last();
            bool addBusiness = ((lastBusiness == null) || (lastConflict.FamilyMemberBusinessWithEthicsList.Last().Business != lastBusiness.Business));
            if (addBusiness) {
                AddBusiness(reader, ethicsInfos.Last());
                //ethicsInfos.Last().FamilyMemberBusinessWithEthicsList.Last();
                lastBusiness = ethicsInfos.Last().FamilyMemberBusinessWithEthicsList.Last();
            }

            lastBusiness.EthicsDocuments.Add(new EthicsDocumentForJson(
                reader["EthicsDocument"].ToString()
                , reader["EthicsDocumentDate"].ToString()
                , reader["EthicsDocumentLink"].ToString()
                )); 
        }

        private static void AddConflict(SqlDataReader reader, List<FamilyMemberBusinessEthicsForConflict> ethicsInfos) {
            ethicsInfos.Add(new FamilyMemberBusinessEthicsForConflict(reader["ConflictID"].ToString()));
        }

        private static void AddBusiness(SqlDataReader reader, FamilyMemberBusinessEthicsForConflict conflict) {
            conflict.FamilyMemberBusinessWithEthicsList.Add(new FamilyMemberBusinessWithEthics(
                reader["FamilyMember"].ToString() 
                , reader["Description"].ToString() 
                , reader["Business"].ToString() 
                , reader["ConflictStatus"].ToString() 
            ));
        }
    }
}

