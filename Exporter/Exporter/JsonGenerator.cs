using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Conflicts;


namespace Phase2 {

    public class FamilyMemberBusinessWithEthics {
        public string FamilyMember;
        public string Description;
        public string Business;
        public string ConflictStatus;
        public List<EthicsDocument> EthicsDocuments;

        public FamilyMemberBusinessWithEthics(string familyMember, string description, string business, string conflictStatus) {
            FamilyMember = familyMember;
            Description = description;
            Business = business;
            ConflictStatus = conflictStatus;

            EthicsDocuments = new List<EthicsDocument>();
        }
    }

    class FamilyMemberBusinessEthicsForConflict {
        public string conflictId;
        public List<FamilyMemberBusinessWithEthics> FamilyMemberBusinessWithEthicsList;
    }
    


    public class JsonGenerator {

        public static void Run(string path) {
            var stories = MakeStories();
            WriteStoryJson(path, stories);

            List<FamilyMemberBusinessEthicsForConflict> familyMemberBusinessEthics = MakeEthicsInfos();
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

            var storyStrings = new List<String>();
            foreach (Conflicts.Story story in stories)
                storyStrings.Add(story.ToJson());

            var strings = new StringBuilder();
            strings.Append("[");
            strings.Append(String.Join(",", storyStrings.ToArray()));
            strings.Append("]");

            var output = strings.ToString();
            var err = output.Substring (1730, 240); // 1944
            
            System.IO.File.WriteAllText(path + "stories2.json", output);
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

            var conflictID = reader["ConflictId"].ToString();
            var business = reader["Business"].ToString();

            bool addConflict = false;
            bool addBusiness = false;

            if (ethicsInfos.Count == 0) {

            }
            
        //    public string FamilyMember;
        //    public string Description;
        //    public string Business;
        //    public string ConflictStatus;
        //}

        //public class EthicsDocument {
        //    public string FamilyMember;
        //    public string Title;
        //    public string Link;
        //    public DateTime Date;
        }
    }
}

