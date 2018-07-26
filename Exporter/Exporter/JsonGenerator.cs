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

        public EthicsDocumentForJson(string name, string date, string link) {
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
        public List<BusinessOwnershipForJson> ownerships;

        public FamilyMemberBusinessWithEthics(string familyMember, string description, string business, string conflictStatus) {
            this.familyMember = familyMember;
            this.description = description;
            this.business = business;
            this.conflictStatus = conflictStatus;

            ethicsDocuments = new List<EthicsDocumentForJson>();
            ownerships = new List<BusinessOwnershipForJson>();
        }
    }

    public class FamilyMemberBusinessEthicsForConflict {
        public string conflictId { get; set; }
        public string conflict { get; set; }
        public string conflictDescription { get; set; }
        public List<FamilyMemberBusinessWithEthics> familyMemberEthics;

        public FamilyMemberBusinessEthicsForConflict(string conflictId, string conflict, string conflictDescription) {
            this.conflictId = conflictId;
            this.conflict = conflict;
            this.conflictDescription = conflictDescription;
            familyMemberEthics = new List<FamilyMemberBusinessWithEthics>();
        }
    }


    public class BusinessOwnershipForJson {
        public string owneeId { get; set; }
        public string owner { get; set; }
        public string percentage { get; set; }

        public BusinessOwnershipForJson(string owneeId, string owner, string percentage) {
            this.owneeId = owneeId;
            this.owner = owner;
            this.percentage = percentage;
        }
    }

    // Everything for the conflict details page
    // ID, Name, Slug, Description
    // List of Stories
    // Ethics Info
    public class ConflictJson {
        private string slug { get; set; }

        public string name { get; set; }
        public string description { get; set; }

        public List<Conflicts.Story> stories { get; set; }

        public List<FamilyMemberBusinessEthicsForConflict> ethics { get; set; }

        public string GetSlug() {
            return slug;
        }


        public ConflictJson(string name, string description, string slug, 
            List<Conflicts.Story> stories, 
            List<FamilyMemberBusinessEthicsForConflict> ethics ) {

            this.name = name;
            this.description = description;
            this.slug = slug;

            this.stories = stories;
            this.ethics = ethics;
        }
    }


    public class ConflictSearchJson {
        public string name { get; set; }
        public string description { get; set; }
        public string slug { get; set; }
        public string stories { get; set; }
        public string lastStory { get; set; }
        public string familyMember { get; set; }
        
        public ConflictSearchJson(string name, string description, string slug,  string stories, string lastStory, string familyMember) {
            this.name = name;
            this.description = description;
            this.slug = slug;
            this.stories = stories;
            this.lastStory = lastStory;
            this.familyMember = familyMember;
        }
    }




    public class JsonGenerator {

        public static string ConnectionString = "Server=SCOTT-PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";

        private static Dictionary<string, string> businesses;



        // New Stuff
        public static void RunConflicts(string path) {

            var allStories = MakeStories();

            var conflictSearches = MakeConflictSearchJsons();

            // Temporarily get rid of these duplicates
            var filteredStories = new List<Conflicts.Story>();
            foreach (Conflicts.Story s in allStories) {
                if ((s.conflict != "Trump Organization LLC D/B/A The Trump Organization") &&
                   (s.conflict != "") &&
                   (s.conflict != "Trump International Hotel DC"))
                    filteredStories.Add(s);
            }

            var filteredConflicts = new List<ConflictSearchJson>();
            foreach (ConflictSearchJson con in conflictSearches) {
                if (con.name != "Trump Organization LLC D/B/A The Trump Organization")
                    filteredConflicts.Add(con);
            }

            WriteConflictSearchJson(path, filteredConflicts);

            WriteStoryJson(path, filteredStories);
            WriteStoryCsv(path, filteredStories);

            List<FamilyMemberBusinessEthicsForConflict> ethicsForConflict = MakeEthicsInfos();
            // ugh - add the ownerships to all businesses as a separate step
            AddOwnersips(ethicsForConflict);

            var conflicts = new List<ConflictJson>();
            var reader = SqlUtil.Query("SELECT ID, Name, Description, Slug FROM Conflict");
            while (reader.Read()) {
                var conflictId = reader["ID"].ToString();

                List<Conflicts.Story> stories = new List<Conflicts.Story>();
                foreach (Conflicts.Story story in filteredStories)
                    if (story.GetID() == conflictId)
                        stories.Add(story);

                var ethicsList = new List<FamilyMemberBusinessEthicsForConflict>();
                foreach (FamilyMemberBusinessEthicsForConflict ethics in ethicsForConflict)
                    if (ethics.conflictId == conflictId)
                        ethicsList.Add(ethics);

                conflicts.Add(
                    new ConflictJson(
                        reader["Name"].ToString()
                        , reader["Description"].ToString()
                        , reader["Slug"].ToString()
                        , stories
                        , ethicsList
                        )
                    );
            }
            WriteConflictJson(path, conflicts);
            //WriteEthicsJson(path);
        }

        private static void WriteConflictJson(string path, List<ConflictJson> conflicts) {
            string fullPath = path + "conflicts\\";
            try {
                Directory.Delete(fullPath, true);
            }
            catch (Exception e) {
            }
            Directory.CreateDirectory(fullPath);

            foreach (ConflictJson conflict in conflicts) {
                string json = JsonConvert.SerializeObject(conflict);
                var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();

                System.IO.File.WriteAllText(fullPath + conflict.GetSlug() + ".json", niceJson);
            }
        }


        private static List<ConflictSearchJson> MakeConflictSearchJsons() {
            var conflicts = new List<ConflictSearchJson>();

            string query = "SELECT * FROM ConflictView";
            using (SqlConnection conn = new SqlConnection(JsonGenerator.ConnectionString)) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                        conflicts.Add(new ConflictSearchJson(
                            reader["Conflict"].ToString()
                            , reader["Description"].ToString()
                            , reader["Slug"].ToString()
                            , reader["Stories"].ToString()
                            , reader["LastStory"].ToString()
                            , reader["FamilyMember"].ToString()
                        )
                    );
                    Console.WriteLine(conflicts.Count.ToString() + " Conflicts");
                }
            }
            return conflicts;
        }

        //private static void WriteEthicsJson(string path) { 
        //    List<FamilyMemberBusinessEthicsForConflict> ethicsForConflict = MakeEthicsInfos();

        //    // ugh - add the ownerships to all businesses as a separate step
        //    AddOwnersips(ethicsForConflict);

        //    WriteEthicsInfosToJson(path, ethicsForConflict);
        //}

        // End New Stuff


        public static void Run(string path) {

                var stories = MakeStories();

                // Temporarily get rid of these duplicates
                var filteredStories = new List<Conflicts.Story>();
                foreach (Conflicts.Story s in stories) {

                    Console.WriteLine(s.conflict);

                    if ((s.conflict != "Trump Organization LLC D/B/A The Trump Organization") &&
                       (s.conflict != "") &&
                       (s.conflict != "Trump International Hotel DC"))
                        filteredStories.Add(s);
                }

                WriteStoryJson(path, filteredStories);
                WriteStoryCsv(path, filteredStories);

                List<FamilyMemberBusinessEthicsForConflict> ethicsForConflict = MakeEthicsInfos();

                // ugh - add the ownerships to all businesses as a separate step
                AddOwnersips(ethicsForConflict);

                WriteEthicsInfosToJson(path, ethicsForConflict);    
            }

            private static void AddOwnersips(List<FamilyMemberBusinessEthicsForConflict> businessesForConflict) {
                List<BusinessOwnershipForJson> ownerships = MakeBusinessOwnerships();

                foreach (FamilyMemberBusinessEthicsForConflict b in businessesForConflict) {
                    foreach (FamilyMemberBusinessWithEthics bus in b.familyMemberEthics) {
                        foreach (BusinessOwnershipForJson ownership in ownerships) {
                            var busId = IdForBusinessName(bus.business);
                            if (busId == ownership.owneeId)
                                bus.ownerships.Add(ownership);
                        }
                    }
                }
            }

            private static void WriteEthicsInfosToJson(string path, List<FamilyMemberBusinessEthicsForConflict> ethicsForConflicts) {
                string fullPath = path + "ethics\\";
                try {
                    Directory.Delete(fullPath, true);
                }
                catch (Exception e) {
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

                //string connString = "Server=SCOTT-PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";
                string query = "SELECT * FROM StoryConflictView ORDER BY Date DESC";
                using (SqlConnection conn = new SqlConnection(JsonGenerator.ConnectionString)) {
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
                    , reader["Slug"].ToString() 
                    , reader["ConflictStatus"].ToString() // Category
                    //, reader["ConflictNotes"].ToString() // Notes
                    //, reader["ConflictUpdateDate"].ToString() // DateChanged
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
                System.IO.File.WriteAllText(path + "stories.json", niceJson);
            }


            private static void WriteConflictSearchJson(string path, List<ConflictSearchJson> conflicts) {
                string json = JsonConvert.SerializeObject(conflicts);
                var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();
                System.IO.File.WriteAllText(path + "conflicts.json", niceJson);
            }

            private static void WriteStoryCsv(string path, List<Conflicts.Story> stories) {
                var storyStrings = new List<String>();
                storyStrings.Add(Conflicts.Story.CsvHeader());
                foreach (Conflicts.Story story in stories)
                    storyStrings.Add(story.ToCsv());

                var strings = new StringBuilder();
                strings.Append(String.Join("\r\n", storyStrings.ToArray()));
                System.IO.File.WriteAllText(path + "stories.csv", strings.ToString());

                //string json = JsonConvert.SerializeObject(stories);
                //var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();
                //System.IO.File.WriteAllText(path + "stories.csv", niceJson);
            }

            private static List<FamilyMemberBusinessEthicsForConflict> MakeEthicsInfos() {
                var ethics = new List<FamilyMemberBusinessEthicsForConflict>();
                //string connString = "Server=PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";
                string query = "SELECT * FROM BusinessConflictView ORDER BY ConflictID, FamilyMember, Business, EthicsDocument";
                using (SqlConnection conn = new SqlConnection(JsonGenerator.ConnectionString)) {
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

            private static string IdForBusinessName(string name) {
                if (businesses == null) {
                    businesses = new Dictionary<string, string>();
                    string query = "SELECT ID, Name FROM Business";
                    using (SqlConnection conn = new SqlConnection(JsonGenerator.ConnectionString)) {
                        using (SqlCommand cmd = new SqlCommand(query, conn)) {
                            cmd.CommandType = CommandType.Text;
                            conn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                                businesses.Add(reader["Name"].ToString(), reader["ID"].ToString());
                        }
                    }
                }
                return businesses[name];
            }

            private static List<BusinessOwnershipForJson> MakeBusinessOwnerships() {
                var ownerships = new List<BusinessOwnershipForJson>();

                //string connString = "Server=PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";
                string query = "SELECT * FROM BusinessOwnershipView ORDER BY owneeId, ownershipPercentage DESC";
                using (SqlConnection conn = new SqlConnection(JsonGenerator.ConnectionString)) {
                    using (SqlCommand cmd = new SqlCommand(query, conn)) {
                        cmd.CommandType = CommandType.Text;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                            ownerships.Add(new BusinessOwnershipForJson(
                                reader["OwneeID"].ToString()
                                , reader["Owner"].ToString()
                                , reader["OwnershipPercentage"].ToString()
                            )
                        );
                    }
                }
                return ownerships;
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
                    lastBusiness = (ethicsInfos.Last().familyMemberEthics.Count == 0) ? null : ethicsInfos.Last().familyMemberEthics.Last();
                bool addBusiness = ((lastBusiness == null) || (lastConflict.familyMemberEthics.Last().business != business));
                if (addBusiness) {
                    AddBusiness(reader, ethicsInfos.Last());
                    lastBusiness = ethicsInfos.Last().familyMemberEthics.Last();
                }

                lastBusiness.ethicsDocuments.Add(new EthicsDocumentForJson(
                    reader["EthicsDocument"].ToString()
                    , reader["EthicsDocumentDate"].ToString()
                    , reader["EthicsDocumentLink"].ToString()
                    ));

                //lastBusiness.ownerships.Add(new BusinessOwnershipForJson("1", "1", "1"));
            }

            private static void AddConflict(SqlDataReader reader, List<FamilyMemberBusinessEthicsForConflict> ethicsInfos) {
                ethicsInfos.Add(new FamilyMemberBusinessEthicsForConflict(
                    reader["ConflictID"].ToString()
                    , reader["Conflict"].ToString()
                    , reader["ConflictDescription"].ToString()));
            }

            private static void AddBusiness(SqlDataReader reader, FamilyMemberBusinessEthicsForConflict conflict) {
                conflict.familyMemberEthics.Add(new FamilyMemberBusinessWithEthics(
                    reader["FamilyMember"].ToString()
                    , reader["Description"].ToString()
                    , reader["Business"].ToString()
                    , reader["ConflictStatus"].ToString()
                ));
            }
        }
    }


