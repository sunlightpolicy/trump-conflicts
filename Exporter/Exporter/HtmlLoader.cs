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

        public Source(string name, string link, DateTime date) {
            Name = name;
            Link = link;
            Date = date;
        }

        public string ToJson() {
            return "{ " +
                 "\"name\": \"" + Util.RemoveQuotes(Name) + "\"," +
                 "\"link\": \"" + Util.RemoveQuotes(Link) + "\"," +
                 //"date=\"" + Date.ToString("d") + "\"" +
                 //"\"date\": \"" + String.Format("{0:MM/dd/yyyy}", Date) + "\"" +
                 "\"date\": \"" + Date.ToString("d") + "\"" +
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


    public class HtmlLoader {

        public HtmlLoader(String path) {

            var parentsFile = "President Donald J. Trump and FLOTUS.html";
            var childrenFile = "Donald Trump Jr., Eric Trump, Ivanka Trump & Jared Kushner.html";
            
            var conflicts = new List<Conflict>();
            ImportPage(conflicts, path + "\\" + parentsFile);
            ImportPage(conflicts, path + "\\" + childrenFile);

            WriteJson(conflicts, "c:\\trump-conflicts\\data\\conflicts.json");
            Console.WriteLine(conflicts.Count.ToString() + " total conflicts");
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

            var source = new Source(name, link, DateTime.Now);

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
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
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
                return string.Empty;

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
}
