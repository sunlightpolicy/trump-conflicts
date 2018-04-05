using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;


namespace Conflicts {

    public class Source {
        public string Name;
        public string Link;
        public DateTime Date;
        public string Headline;

        public Source(string name, string link, DateTime date) {
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

    public class Conflict {
        
        public string Description;
        public string FamilyMember;
        public string ConflictingEntity;
        public string Category;
        public string Notes;
        public DateTime DateChanged;
        public List<Source> Sources;

        public string Id;

        public Conflict(string description, string familyMember, string conflictingEntity, string category, string notes, DateTime dateChanged) {
            Description = description;
            FamilyMember = familyMember;
            ConflictingEntity = conflictingEntity;
            Category = category;
            Notes = notes;
            DateChanged = dateChanged;

            Sources = new List<Source>();
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
            foreach (Source source in Sources)
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


    public class Story {

        public string description;
        public string familyMember;
        public string conflict;
        public string category;
        public string notes;
        public DateTime dateChanged;

        public string mediaOutlet;
        public string link;
        public DateTime sourceDate;
        public string headline;

        public string conflictId;

        public Story(Conflict conflict, Source source) {
            description = conflict.Description;
            familyMember = conflict.FamilyMember;
            this.conflict = conflict.ConflictingEntity;
            category = conflict.Category;
            notes = conflict.Notes;
            dateChanged = conflict.DateChanged;

            mediaOutlet = source.Name;
            link = source.Link;
            sourceDate = source.Date;
            headline = source.Headline;
        }


        public Story (
            string description
            , string familyMember
            , string conflict
            , string category
            , string notes
            , string dateChanged

            , string mediaOutlet  // was source
            , string link
            , string date
            , string headline
            
            , string conflictId = "") {
            
            this.description = description;
            this.familyMember = familyMember;
            this.conflict = conflict;
            this.category = category;
            this.notes = notes;
            this.dateChanged = Convert.ToDateTime(dateChanged);

            this.mediaOutlet = mediaOutlet;
            this.link = link;
            sourceDate = Convert.ToDateTime(date);
            this.headline = headline;

            this.conflictId = conflictId;
        }

        public string ToJson() {
            return
                "{" +
                "\"conflictId\": \"" + conflictId + "\"," +
                "\"description\": \"" + Util.RemoveQuotes(description) + "\"," +
                "\"familyMember\": \"" + Util.RemoveQuotes(familyMember) + "\"," +
                "\"conflict\": \"" + Util.RemoveQuotes(conflict) + "\"," +
                "\"category\": \"" + Util.RemoveQuotes(category) + "\", " +
                "\"notes\": \"" + Util.RemoveQuotes(notes) + "\"," +
                "\"dateChanged\": \"" + String.Format("{0:MM/dd/yyyy}", dateChanged) + "\"," +

                "\"mediaOutlet\": \"" + Util.RemoveQuotes(mediaOutlet) + "\"," +
                "\"link\": \"" + Util.RemoveQuotes(link) + "\"," +
                "\"sourceDate\": \"" + String.Format("{0:MM/dd/yyyy}", sourceDate) + "\"," +
                "\"headline\": \"" + Util.RemoveQuotes(headline) + "\"" +
                "}";
        }


        // CSV does not do anything with the list of stories! Put in another CSV?? 
        public static string CsvHeader() {
            return
                "description," +
                "familyMember," +
                "conflict," +
                "category," +
                "notes," +
                "dateChanged," +
                "mediaOutlet," +
                "link," +
                "sourceDate," +
                "headline";
        }

        public string ToCsv() {
            return
                "\"" + Util.RepeatQuotes(description) + "\"," +
                familyMember + "," +
                "\"" + Util.RepeatQuotes(conflict) + "\"," +
                category + ", " +
                "\"" + Util.RepeatQuotes(notes) + "\"," +
                String.Format("{0:MM/dd/yyyy}", dateChanged) + "," + 

                "\"" + Util.RemoveQuotes(mediaOutlet) + "\"," +
                "\"" + Util.RemoveQuotes(link) + "\"," +
                "\"" + String.Format("{0:MM/dd/yyyy}", sourceDate) + "\"," +
                "\"" + Util.RemoveQuotes(headline) + "\"";
        }
    }


        public class HtmlLoader {

        public HtmlLoader(String path) {

            var parentsFile = "President Donald J. Trump and FLOTUS.html";
            var childrenFile = "Donald Trump Jr., Eric Trump, Ivanka Trump & Jared Kushner.html";
            
            var conflicts = new List<Conflict>();
            ImportPage(conflicts, path + "\\" + parentsFile);
            AddParentHeadlines(conflicts);

            int firstChildrenItem = conflicts.Count;
            ImportPage(conflicts, path + "\\" + childrenFile);
            AddChildrenHeadlines(conflicts, firstChildrenItem);

            WriteJson(conflicts, "c:\\trump-conflicts\\data\\conflicts.json");
            WriteStoriesJson(conflicts, "c:\\trump-conflicts\\data\\stories.json");
            WriteStoriesCsv(conflicts, "c:\\trump-conflicts\\data\\trump_conflicts_of_interest.csv");

            WriteSql(conflicts, "c:\\trump-conflicts\\Exporter\\Exporter\\Db\\");
            Console.WriteLine(conflicts.Count.ToString() + " total conflicts");
        }

        private void AddParentHeadlines(List<Conflict> conflicts){
            conflicts[0].Sources[0].Headline = "Bank reported Trump lawyer’s payment to Stormy Daniels as suspicious";
            conflicts[0].Sources[1].Headline = "DOES STORMY DANIELS HAVE \"IMAGES\" OF DONALD TRUMP?";
            conflicts[0].Sources[2].Headline = "Stormy Daniels suit could back Trump into a corner";

            conflicts[1].Sources[0].Headline = "Israel-focused charity praises Trump - and pays him - at Mar-a-Lago gala";

            conflicts[2].Sources[0].Headline = "Trump officials fight eviction from Panama hotel they manage";
            conflicts[2].Sources[1].Headline = "Ethics experts say their 'fear has been realized' as Trump faces one of his most consequential conflicts of interest yet";
            conflicts[2].Sources[2].Headline = "Judge, police help oust Trump Hotels from Panama property";
        }

        private void AddChildrenHeadlines(List<Conflict> conflicts, int start) {
            conflicts[start].Sources[0].Headline = "Exclusive: FBI counterintel investigating Ivanka Trump business deal";

            conflicts[start + 1].Sources[0].Headline = "Kushner's Family Business Received Loans After White House Meetings";
            conflicts[start + 1].Sources[1].Headline = "How*" +
                "vv Kushner's Finances Could Be Potential Conflicts Of Interest";
            conflicts[start + 1].Sources[2].Headline = "How Kushner's Finances Could Be Potential Conflicts Of Interest";

            conflicts[start + 2].Sources[0].Headline = "Kushner’s Family Business Received Loans After White House Meetings";
            conflicts[start + 2].Sources[1].Headline = "How Kushner's Finances Could Be Potential Conflicts Of Interest";
            conflicts[start + 2].Sources[2].Headline = "How Kushner's Finances Could Be Potential Conflicts Of Interest";

            conflicts[start + 3].Sources[0].Headline = "Trump Jr. to give foreign policy speech while on 'unofficial' business trip to India";
            conflicts[start + 3].Sources[1].Headline = "Ad blitz heralds Donald Trump Jr.'s visit to India";
            conflicts[start + 3].Sources[2].Headline = "Trump Jr. says missing out on India deals because of father's self-imposed curbs";
        }


        private void WriteJson(List<Conflict> conflicts, string file) {

            var conflictStrings = new List<String>();
            foreach (Conflict conflict in conflicts)
                conflictStrings.Add(conflict.ToJson());
            
            var strings = new StringBuilder();
            strings.Append("[");
            strings.Append(String.Join(",", conflictStrings.ToArray()));
            strings.Append("]");

            System.IO.File.WriteAllText(file, strings.ToString());
        }

        private void WriteStoriesJson(List<Conflict> conflicts, string file) {

            int news = 0;

            var storyStrings = new List<String>();
            foreach (Conflict conflict in conflicts) {
                foreach (Source source in conflict.Sources) { 
                    storyStrings.Add(new Story(conflict, source).ToJson());

                    if (source.Name != "Office of Government Ethics") {
                        Console.WriteLine(source.Date.ToString());
                        news++;
                    }
                }
            }
            Console.WriteLine("NEWS: " + news.ToString());

            var strings = new StringBuilder();
            strings.Append("[");
            strings.Append(String.Join(",", storyStrings.ToArray()));
            strings.Append("]");

            System.IO.File.WriteAllText(file, strings.ToString());
            Console.WriteLine(storyStrings.Count.ToString() + " total stories");
        }

        private void WriteStoriesCsv(List<Conflict> conflicts, string file) {
            var storyStrings = new List<String>();
            storyStrings.Add(Story.CsvHeader()); 

            foreach (Conflict conflict in conflicts) {
                foreach (Source source in conflict.Sources) {
                    storyStrings.Add(new Story(conflict, source).ToCsv());
                }
            }
            var strings = new StringBuilder();
            strings.Append(String.Join("\r\n", storyStrings.ToArray()));
            System.IO.File.WriteAllText(file, strings.ToString());
        }

        private void ImportPage(List<Conflict> conflicts, string file) {
            
            string text = File.ReadAllText(file, Encoding.UTF8);

            int rowNum = 0;
            int max = 100000;
            var rows = text.Split(new[] { "<tr " }, StringSplitOptions.None);
            foreach (string row in rows) {
                if (rowNum < max) {
                    AddConflict(conflicts, row, file, rowNum);
                    rowNum++;
                }
            }
            Console.WriteLine(conflicts.Count.ToString() + " conflicts");
        }

        private void AddConflict(List<Conflict> conflicts, string row, string file, int rowNum) {
            row = row.Replace("<td></td>", "<td ></td>");
            var cols = row.Split(new[] { "<td " }, StringSplitOptions.None);

            if (cols.Length < 2)
                return;
            if (cols[1].Contains("></td>")) 
                return;
            if (cols[1].Contains("Description</td"))
                return;

                var conflict = new Conflict(
                Description(cols[1]),
                FamilyMember(cols[2]),
                ConflictingEntity(cols[3]),
                Category(cols[4]),
                Notes(cols[5]),
                DateChanged(cols[12]));

            try {
                AddSource(conflict, cols[6], cols[7]);
                AddSource(conflict, cols[8], cols[9]);
                AddSource(conflict, cols[10], cols[11]);
            } catch (Exception e) {
                Console.WriteLine("LINK PROBLEM: " + Path.GetFileNameWithoutExtension(file) + " at " + rowNum.ToString());
                return;
            }

            conflicts.Add(conflict);
        }

        private void AddSource(Conflict conflict,  string text, string date) {
            var fields = text.Split(new[] { " href=" }, StringSplitOptions.None);
            if (fields.Length < 2)
                return;

            var linkAndName = fields[1].Split('>');
            
            var name = linkAndName[1].Replace("</a", "");
            var link = linkAndName[0].Replace("\"", "");

            var dateStr = date.Split('>')[1].Replace("</td", "");
            var dte = GetDate(dateStr);

            var source = new Source(name, link, dte);

            conflict.Sources.Add(source);
        }

        private DateTime GetDate(string dte) {

            dte = dte
                .Replace("Sept.", "September");

            DateTime date = Convert.ToDateTime(dte.Replace(".", ""));
            
            return date;
        }


        private string Description(string txt) {
            var subs = txt.Split('>');
            return 
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
        }

        private string FamilyMember(string txt) {
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


        private void WriteSql(List<Conflict> conflicts, string path) {

            WriteConflictingEntity(conflicts, path);
            WriteSource(conflicts, path);
            WriteConflict(conflicts, path);
            WriteStory(conflicts, path);
        }


        private void WriteConflictingEntity(List<Conflict> conflicts, string path) {
            var conflictingEntities = new List<string>();
            
            foreach (Conflict conflict in conflicts) {
                if (!conflictingEntities.Contains(conflict.ConflictingEntity))
                    conflictingEntities.Add(conflict.ConflictingEntity);
            }
            var conflictStrings = new List<String>();

            var strings = new StringBuilder();
            strings.Append("USE Trump\r\n");
            strings.Append("GO\r\n");
            foreach (String conflictingEntity in conflictingEntities)
                strings.Append("INSERT INTO ConflictingEntity VALUES ('" + conflictingEntity + "' , GetDate(), 1)\r\n");

            System.IO.File.WriteAllText(path + "4 ConflictingEntities.sql", strings.ToString());
        }

        private void WriteSource(List<Conflict> conflicts, string path) {
            var sourceNames = new List<string>();

            // Source
            foreach (Conflict conflict in conflicts) {
                foreach (Source source in conflict.Sources)
                    if (!sourceNames.Contains(source.Name))
                        sourceNames.Add(source.Name);
            }
            var sourceStrings = new List<String>();

            var strings = new StringBuilder();
            strings.Append("USE Trump\r\n");
            strings.Append("GO\r\n");
            foreach (String source in sourceNames)
                strings.Append("INSERT INTO Source VALUES ('" + source + "' , GetDate(), 1)\r\n");

            System.IO.File.WriteAllText(path + "5 Sources.sql", strings.ToString());
        }

        private void WriteConflict(List<Conflict> conflicts, string path) {
            var conflictStrings = new List<String>();

            var strings = new StringBuilder();
            strings.Append("USE Trump\r\n");
            strings.Append("GO\r\n");
            foreach (Conflict conflict in conflicts)
                strings.Append("INSERT INTO Conflict VALUES (" + 
                    "(SELECT ID FROM ConflictingEntity WHERE Name = '" + conflict.ConflictingEntity + "'), " +
                    "(SELECT ID FROM FamilyMember WHERE Name = '" + conflict.FamilyMember + "'), " +
                    "(SELECT ID FROM Category WHERE Name = '" + conflict.Category + "'), " +
                    "'" + conflict.DateChanged + "', " +
                    "GetDate(), 1)\r\n");
            System.IO.File.WriteAllText(path + "7 Conflicts.sql", strings.ToString());
        }


        private void WriteStory(List<Conflict> conflicts, string path) {
            var story = new List<String>();

            var strings = new StringBuilder();
            strings.Append("USE Trump\r\n");
            strings.Append("GO\r\n");
            int conflictId = 1;
            foreach (Conflict conflict in conflicts) {
                foreach (Source source in conflict.Sources) { 
                    strings.Append("INSERT INTO Story VALUES (" +
                        "(SELECT ID FROM source WHERE Name = '" + source.Name + "'), " +
                        conflictId.ToString() +
                        ", '" + conflict.DateChanged + "', " +
                        "GetDate(), 1)\r\n");
                }
                conflictId++;
            }
            System.IO.File.WriteAllText(path + "8 Stories.sql", strings.ToString());
        }
    }
}
