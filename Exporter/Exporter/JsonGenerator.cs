using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Conflicts;


namespace Phase2 {

    public class JsonGenerator {

        public static void Run(string path) {
           var stories = MakeStories(path);
            WriteJson(path, stories);
        }

        private static List<Conflicts.Story> MakeStories(string path) {
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
            );
            stories.Add(story);
        }

        private static void WriteJson(string path, List<Conflicts.Story> stories) {

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
    }
}
