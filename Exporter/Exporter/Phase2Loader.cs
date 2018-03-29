using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;


namespace Phase2 {

    enum Col {
        Parent3Pct = 1,
        Parent3,
        Parent2Pct,
        Parent2,
        Parent1Pct,
        Parent1,
        ConflictName,
        ConflictDescription,
        ConflictingEntity,
        FamilyMember,
        FamilyRelationshipDescription,
        FamilyMemberConflictStatus,
        Source1,
        Source1Date,
        Source2,
        Source2Date,
        Source3,
        Source3Date,
        Notes,
        DatePublished
    };

    //public class Table {
    //    public string Id;
    //}

    //public class BusinessTable : Table {
    //    public string Name;
    //} 

    public class Conflict {
        public string Name;
        public string Description;
        public string Notes;
        public DateTime DateChanged;
    }

    public class FamilyMemberBusiness {
        public string FamilyMember;
        public string Description;
        public string Business;
        public string ConflictStatus;
    }
    
    public class Story {
        public string Conflict;
        public string Name;
        public string Link;
        public DateTime Date;
        public string Headline;

        public Story(string conflict, string name, string link, DateTime date) {
            Conflict = conflict;
            Name = name;
            Link = link;
            Date = date;

            Headline = "";
        }

        public string ToJson() {
            return "{ " +
                 "\"name\": \"" + Util.RemoveQuotes(Name) + "\"," +
                 "\"link\": \"" + Util.RemoveQuotes(Link) + "\"," +
                 //"date=\"" + Date.ToString("d") + "\"" +
                 "\"date\": \"" + String.Format("{0:MM/dd/yyyy}", Date) + "\"" +
                 "}";
        }
    }


    public class ConflictOld {

        public string Description;
        public string FamilyMember;
        public string ConflictingEntity;
        public string Category;
        public string Notes;
        public DateTime DateChanged;
        public List<Story> Sources;

        public ConflictOld(string description, string familyMember, string conflictingEntity, string category, string notes, DateTime dateChanged) {
            Description = description;
            FamilyMember = familyMember;
            ConflictingEntity = conflictingEntity;
            Category = category;
            Notes = notes;
            DateChanged = dateChanged;

            Sources = new List<Story>();
        }

        public string ToJson() {
            return
                "{" +
                "\"description\": \"" + Util.RemoveQuotes(Description) + "\"," +
                "\"familyMember\": \"" + Util.RemoveQuotes(FamilyMember) + "\"," +
                "\"conflictingEntity\": \"" + Util.RemoveQuotes(ConflictingEntity) + "\"," +
                "\"category\": \"" + Util.RemoveQuotes(Category) + "\", " +
                "\"notes\": \"" + Util.RemoveQuotes(Notes) + "\"," +
                "\"dateChanged\": \"" + String.Format("{0:MM/dd/yyyy}", DateChanged) + "\"," +
                SourcesToJson() +
                "}";
        }

        private string SourcesToJson() {

            var sourceStrings = new List<String>();
            foreach (Story source in Sources)
                sourceStrings.Add(source.ToJson());

            var strings = new StringBuilder();
            strings.Append("\"sources\": [");
            strings.Append(String.Join(",", sourceStrings.ToArray()));
            strings.Append("]");

            return strings.ToString();
        }

        private string UpperCaseFirstChar(string str) {
            if (string.IsNullOrEmpty(str)) {
                return string.Empty;
            }
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private string TrumpName(string name) {
            switch (name) {
                case "Donald Trump": return "Donald Sr.";
                case "Donald Trump Jr.": return "Donald Jr.";
                case "Eric Trump": return "Eric";
                case "Ivanka Trump": return "Ivanka";
                case "Jared Kushner": return "Jared";
                case "Melania Trump": return "Melania";
            }
            return name;
        }
    }


    class Phase2Loader {

        Dictionary<string, string> Businesss = new Dictionary<string, string>();
        Dictionary<string, Conflict> Conflicts = new Dictionary<string, Conflict>();

        Dictionary<string, string> MediaOutlets = new Dictionary<string, string>();
        List<Story> Stories = new List<Story>();
        Dictionary<string, string> StoryConflicts = new Dictionary<string, string>();

        List<FamilyMemberBusiness> FamilyMemberBusiness = new List<FamilyMemberBusiness>();


        public Phase2Loader(String path) {
            
            var parentsFile = "Copy of Donald & Melania.html";

            //var childrenFile = "Donald Trump Jr., Eric Trump, Ivanka Trump & Jared Kushner.html";

            var conflicts = new List<Conflict>();
            ImportPage(conflicts, path + "\\" + parentsFile);
            //AddParentHeadlines(conflicts);

            //int firstChildrenItem = conflicts.Count;
            //ImportPage(conflicts, path + "\\" + childrenFile);
            //AddChildrenHeadlines(conflicts, firstChildrenItem);

            //WriteJson(conflicts, "c:\\trump-conflicts\\data\\conflicts.json");
            //WriteStoriesJson(conflicts, "c:\\trump-conflicts\\data\\stories.json");
            //WriteStoriesCsv(conflicts, "c:\\trump-conflicts\\data\\trump_conflicts_of_interest.csv");

            //WriteSql(conflicts, "c:\\trump-conflicts\\Exporter\\Exporter\\Db\\");
            //Console.WriteLine(conflicts.Count.ToString() + " total conflicts");

            WriteSql("c:\\trump-conflicts\\Exporter\\Exporter\\db\\Phase2\\");
        }

        private void WriteSql(string path) {
            WriteBusinessScript(path, "4");
            WriteConflictScript(path, "5");

            WriteMediaOutletScript(path, "6");
            WriteStoryScript(path, "7");

            WriteFamilyMemberBusinessScript(path, "8");
        }

        private void ImportPage(List<Conflict> conflicts, string file) {

            string text = File.ReadAllText(file, Encoding.UTF8);

            int rowNum = 0;
            int max = 100000;
            var rows = text.Split(new[] { "<tr " }, StringSplitOptions.None);
            foreach (string row in rows) {
                if (rowNum < max && rowNum > 1) {

                    string theRow = row.Replace("<td></td>", "<td ></td>");
                    var cols = theRow.Split(new[] { "<td " }, StringSplitOptions.None);
                    if (cols.Length < 2)
                        return;

                    AddBusinesss(cols);
                    AddConflict(cols);
                    AddStories(cols);
                    AddFamilyMemberBusiness(cols);
                    //AddConflict(conflicts, cols, file, rowNum);
                }
                rowNum++;
            }
            
            Console.WriteLine(Businesss.Count.ToString() + " BusinesUnits");
            Console.WriteLine(Conflicts.Count.ToString() + " Conflicts");
            //Console.WriteLine(conflicts.Count.ToString() + " conflicts");
        }

        private void AddConflict(string[] cols) {
            var name = ConflictingEntity(cols[(int)Col.ConflictName]);
            if (name == "")
                return;
            if (Conflicts.ContainsKey(name))
                return;
            
            var description = Description(cols[(int)Col.ConflictDescription]);
            var note = Notes(cols[(int)Col.Notes]);
            var date = DateChanged(cols[(int)Col.DatePublished]);
            
            var conflict = new Conflict();
            conflict.Name = name;
            conflict.Description = description;
            conflict.Notes = note;
            conflict.DateChanged = date;

            Conflicts.Add(name, conflict);
        }

        private void AddStories(string[] cols) {
            AddStory(cols[(int)Col.ConflictName], cols[(int)Col.Source1], cols[(int)Col.Source1Date]);
            AddStory(cols[(int)Col.ConflictName], cols[(int)Col.Source2], cols[(int)Col.Source2Date]);
            AddStory(cols[(int)Col.ConflictName], cols[(int)Col.Source3], cols[(int)Col.Source3Date]);
        }

        private void AddStory(string conflictingEntityCol, string linkCol, string dateCol) {
            var fields = linkCol.Split(new[] { " href=" }, StringSplitOptions.None);
            if (fields.Length < 2)
                return;

            var busUnit = BusUnit(conflictingEntityCol);

            var linkAndName = fields[1].Split('>');
            var name = linkAndName[1].Replace("</a", "");
            var link = linkAndName[0].Replace("\"", "");

            var dateStr = dateCol.Split('>')[1].Replace("</td", "");
            var dte = GetDate(dateStr);

            if ((name != "") && (!MediaOutlets.ContainsKey(name)))
                MediaOutlets.Add(name, name);

            if (name != "") {
                var story = new Story(busUnit, name, link, dte); 
                Stories.Add(story);
            }
        }


        private void AddFamilyMemberBusiness(string[] cols) {
            
            var x = new FamilyMemberBusiness();
            x.Business = ConflictingEntity(cols[(int)Col.ConflictingEntity]);
            x.ConflictStatus = Category(cols[(int)Col.FamilyMemberConflictStatus]);
            x.FamilyMember = FamilyMember(cols[(int)Col.FamilyMember]);
            x.Description = Description(cols[(int)Col.FamilyRelationshipDescription]);

            if (x.Business != "")
                FamilyMemberBusiness.Add(x);
        }


        private void AddBusinesss(string [] cols) {
            AddBusiness(BusUnit(cols[(int)Col.Parent1]));
            AddBusiness(BusUnit(cols[(int)Col.Parent2]));
            AddBusiness(BusUnit(cols[(int)Col.Parent3]));

            AddBusiness(BusUnit(cols[(int)Col.ConflictingEntity]));
        }

        private void AddBusiness(string unit) {
            if (unit == "")
                return;
            if (!Businesss.ContainsKey(unit))
                Businesss.Add(unit, Businesss.Count.ToString());
        }

        private void WriteBusinessScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (String unit in Businesss.Keys)
                strings.Add("INSERT INTO Business VALUES ('" + unit + "')");

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT Business.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
         }

        private void WriteStoryScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            int storyId = 1;
            foreach (Story story in Stories) {
                strings.Add("INSERT INTO Story VALUES (" +
                    "(SELECT ID FROM MediaOutlet WHERE Name = '" + story.Name + "'), " +
                    "2, " + // StoryStatusUD
                    "'', " + // Headline
                    "'" + story.Date + "', " +
                    "GetDate()," +
                    "1)" // EditorID 
                );

                if (story.Conflict != "") {
                    strings.Add("INSERT INTO StoryConflict VALUES (" +
                        storyId + ", " +
                        "(SELECT ID FROM Conflict WHERE Name = '" + story.Conflict + "'))"
                    );
                }
            }

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT Story and StoryConflict.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }


        private void WriteFamilyMemberBusinessScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (FamilyMemberBusiness x in this.FamilyMemberBusiness) {
                strings.Add("INSERT INTO FamilyMemberBusiness VALUES (" +
                    "(SELECT ID FROM FamilyMember WHERE Name = '" + x.FamilyMember + "'), " +
                    "(SELECT ID FROM Business WHERE Name = '" + x.Business + "'), " +
                    "(SELECT ID FROM FamilyMemberConflictStatus WHERE Name = '" + x.ConflictStatus + "'), " +
                    "'" + x.Description + "')"
                );
            }

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT FamilyMemberBusiness.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }


        private void WriteConflictScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (Conflict conflict in Conflicts.Values)
                strings.Add("INSERT INTO Conflict VALUES ('" +
                    conflict.Name + "', '" +
                    conflict.Description + "', '" +
                    conflict.Notes + "', '" +
                    conflict.DateChanged.ToString() + "', " +
                    "GetDate(), 1)"
                );

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT Conflict.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }

        private void WriteMediaOutletScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (String outlet in MediaOutlets.Keys)
                strings.Add("INSERT INTO MediaOutlet VALUES ('" + outlet + "')");

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT MediaOutlet.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }

        //private void AddConflict(List<Conflict> conflicts, string[] cols, string file, int rowNum) {
        //    //row = row.Replace("<td></td>", "<td ></td>");
        //    //var cols = row.Split(new[] { "<td " }, StringSplitOptions.None);

        //    //if (cols.Length < 2)
        //    //    return;
        //    //if (cols[1].Contains("></td>"))
        //    //    return;
        //    //if (cols[1].Contains("Description</td"))
        //    //    return;

        //    var conflict = new Conflict(
        //    Description(cols[1]),
        //    FamilyMember(cols[2]),
        //    ConflictingEntity(cols[3]),
        //    Category(cols[4]),
        //    Notes(cols[5]),
        //    DateChanged(cols[12]));

        //    try {
        //        AddSource(conflict, cols[6], cols[7]);
        //        AddSource(conflict, cols[8], cols[9]);
        //        AddSource(conflict, cols[10], cols[11]);
        //    }
        //    catch (Exception e) {
        //        Console.WriteLine("LINK PROBLEM: " + Path.GetFileNameWithoutExtension(file) + " at " + rowNum.ToString());
        //        return;
        //    }

        //    conflicts.Add(conflict);
        //}

        //private void AddSource(Conflict conflict, string text, string date) {
        //    var fields = text.Split(new[] { " href=" }, StringSplitOptions.None);
        //    if (fields.Length < 2)
        //        return;

        //    var linkAndName = fields[1].Split('>');

        //    var name = linkAndName[1].Replace("</a", "");
        //    var link = linkAndName[0].Replace("\"", "");

        //    var dateStr = date.Split('>')[1].Replace("</td", "");
        //    var dte = GetDate(dateStr);

        //    var source = new Story(name, link, dte);

        //    conflict.Sources.Add(source);
        //}

        private DateTime GetDate(string dte) {
            dte = dte
                .Replace("Sept.", "September");
            DateTime date = Convert.ToDateTime(dte.Replace(".", ""));
            return date;
        }


        private string BusUnit(string txt) {
            var subs = txt.Split('>');
            return
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
        }

        public string Description(string txt) {
            var subs = txt.Split('>');
            return
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
        }

        public string FamilyMember(string txt) {
            var subs = txt.Split('>');
            return
                subs[1].Replace("</td", "")
                    .TrimEnd()
                    .TrimStart()
                    .Replace("ump, Jr", "ump Jr");
        }

        private string ConflictingEntity(string txt) {
            var subs = txt.Split('>');
            var entity = subs[1].Replace("</td", "").TrimEnd().TrimStart();
            return
                CleanEntity(entity);
        }

        private string CleanEntity(string txt) {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo
                .ToTitleCase(txt.ToLower())
                .Replace("Ny", "NY")
                .Replace("Llc", "LLC")
                .Replace("Nj", "NJ")
                .Replace(" Va", " VA");
        }

        private string Category(string txt) {
            var subs = txt.Split('>');
            var category = subs[1].Replace("</td", "").TrimEnd().TrimStart();

            if (string.IsNullOrEmpty(category))
                return "Active";  // Only one of these

            return char.ToUpper(category[0]) + category.Substring(1);
        }

        private string Notes(string txt) {
            var subs = txt.Split('>');
            return
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
        }

        private DateTime DateChanged(string txt) {
            var subs = txt.Split('>');
            var dte = subs[1].Replace("</td", "").TrimEnd().TrimStart();
            return
                Convert.ToDateTime(dte);
        }   
    }


    public class Util {
        public static string RemoveQuotes(String str) {
            if (str == null)
                return "";
            return str.Replace("\"", "");
        }

        // CSVs need internal quotes to be repeated
        public static string RepeatQuotes(String str) {
            if (str == null)
                return "";
            return str.Replace("\"", "\"\"");
        }
    }
}
