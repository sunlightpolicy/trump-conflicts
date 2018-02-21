using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


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
                 "name=\"" + Util.RemoveQuotes(Name) + "\"," +
                 "link=\"" + Util.RemoveQuotes(Link) + "\"," +
                 "date=\"" + Date.ToString("d") + "\"" +
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
            //AddSource(row, 3);
            //AddSource(row, 5);
            //AddSource(row, 7);
        }

        //{"menu": {
        //    "id": "file",
        //    "value": "File",
        //    "popup": {
        //    "menuitem": [
        //        {"value": "New", "onclick": "CreateNewDoc()"},
        //        {"value": "Open", "onclick": "OpenDoc()"},
        //        {"value": "Close", "onclick": "CloseDoc()"}
        //        ]
        //    }
        //}}

        public string ToJson() {
            return
                "{" +
                "\"description\": \"" + Util.RemoveQuotes(Description) + "\"," +
                "\"familyMember\": \"" + Util.RemoveQuotes(FamilyMember) + "\"," +
                "\"conflictingEntity\": \"" + Util.RemoveQuotes(ConflictingEntity) + "\"," +
                "\"category\": \"" + Util.RemoveQuotes(Category) + "\", " +
                "\"notes\": \"" + Util.RemoveQuotes(Notes) + "\"," +
                "\"dateChanged\": \"" + String.Format("{0:MM/dd/yyyy}", DateChanged) + "\"" +
                "}";
        }

        private void AddSource(string source, string link, DateTime date) {
            Sources.Add(new Source(source, link, date));
        }

        private void AddSource(int row, int col) {
            //string source = "";
            //string link = "";
            //DateTime date = DateTime.Now;
            //if ((cells[row, col].Value != null) || (cells[row, col + 1].Value != null)) {
            //    bool valid = true;

            //    try {  // Test for valid source
            //        source = (String)cells[row, col].Value;
            //        link = GetLink((String)cells[row, col].Formula);
            //    }
            //    catch {
            //        valid = false;
            //        Console.WriteLine("Source at row " + Convert.ToString(row) + ", column " + Convert.ToString(col) +
            //            " not valid");
            //    }

            //    try {  // Test for valid date
            //        date = cells.Worksheet.Workbook.NumberToDateTime((double)cells[row, col + 1].Value);
            //    }
            //    catch {
            //        valid = false;
            //        Console.WriteLine("Date at row " + Convert.ToString(row) + ", column " + Convert.ToString(col + 1) +
            //            " not valid (" + (string)cells[row, col + 1].Value + ")");
            //    }

            //    if (valid)
            //        Sources.Add(new ElmSource(source, link, date));
            //}
        }

        //private string GetLink(string txt) {
        ////  = HYPERLINK("https://www.wsj.com/articles/trump-debts-are-widely-held-on-wall-street-creating-new-potential-conflicts-1483637414", "Wall Street Journal")
        //string link = txt;

        //int first = link.IndexOf("http");
        //link = link.Substring(first, txt.Length - first);

        //int last = link.IndexOf("\",");
        //link = link.Substring(0, last);

        //return link;
        //}
        
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

            //var parentsFile = "Parents.html";
            //var childrenFile = "Children.Html";

            var conflicts = new List<Conflict>();
            ImportPage(conflicts, path + "\\" + parentsFile);
            ImportPage(conflicts, path + "\\" + childrenFile);

            WriteJson(conflicts, path + "\\" + "conflicts.json"); 
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
            //class="s5" dir="ltr"><a target = "_blank" href="https://oge.app.box.com/s/kz4qvbdsbcfrzq16msuo4zmth6rerh1c">Office of Government Ethics</a></td>
            var fields = text.Split(new[] { " href=" }, StringSplitOptions.None);
            if (fields.Length < 2)
                return;

            var linkAndName = fields[1].Split('>');
            
            var name = linkAndName[1].Replace("</a", "");
            var link = linkAndName[0].Replace("\"", "");

            var dateStr = date.Split('>')[1].Replace("</td", "");
            var dte = GetDate(dateStr);

            var source = new Source(name, link, DateTime.Now);

            //link = link.Replace("class="s5" dir="ltr"><a target = "_blank" href="")
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
            return
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
        }

        private string Category(string txt) {
            var subs = txt.Split('>');
            return
                subs[1].Replace("</td", "").TrimEnd().TrimStart();
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
