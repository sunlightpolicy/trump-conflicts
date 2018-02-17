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

        //public string Elm() {
        //    return "{ " +
        //         "name = \"" + Util.RemoveQuotes(Name) + "\",  " +
        //         "link = \"" + Util.RemoveQuotes(Link) + "\",  " +
        //         "date = \"" + Date.ToString("d") + "\" " +
        //         "}";
        //}
    }


    public class Conflict {

        public string Description;
        public string FamilyMember;
        public string ConflictingEntity;
        public string Category;
        public string Notes;
        //public DateTime DateAddedOrEdited;
        //public List<ElmSource> Sources;

        public Conflict(string description) {
            Description = description;


            //this.cells = cells;

            //Description = (string)cells[row, 0].Value;
            //FamilyMember = (string)cells[row, 1].Value;
            //ConflictingEntity = (string)cells[row, 2].Value;
            //Category = (string)cells[row, 9].Value;
            //Notes = (string)cells[row, 10].Value;
            //DateAddedOrEdited = cells.Worksheet.Workbook.NumberToDateTime((double)cells[row, 11].Value);

            //Sources = new List<ElmSource>();
            //AddSource(row, 3);
            //AddSource(row, 5);
            //AddSource(row, 7);
        }



        //public string Json() {
        //    return
        //        "{" + JsonFields() + "}  ";
        //        //"{" + Fields() + Sources() + "}  ";
        //}


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
                "\"description\": \"" + Util.RemoveQuotes(Description) + "\", " +
                "\"familyMemory\": \"" + Util.RemoveQuotes(FamilyMember) + "\", " +
                "\"conflictingEntity\": \"" + Util.RemoveQuotes(ConflictingEntity) + "\", " +
                "\"description\": \"" + Util.RemoveQuotes(Category) + "\", " +
                "\"notes\": \"" + Util.RemoveQuotes(Notes) + "\"" +
                "}";


            //"familyMember = \"" + TrumpName(Util.RemoveQuotes(FamilyMember)) + "\", " +
            //"conflictingEntity = \"" + Util.RemoveQuotes(ConflictingEntity) + "\", " +
            //"category = \"" + UpperCaseFirstChar(Util.RemoveQuotes(Category)) + "\", " +
            //"notes = \"" + Util.RemoveQuotes(Notes) + "\", " +
            //"dateAddedOrEdited = \"" + DateAddedOrEdited.ToString("d") + "\"  "; 
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


        //private string ElmSources() {
        //    List<String> elmSources = new List<String>();
        //    foreach (ElmSource source in Sources)
        //        elmSources.Add(source.Elm());
        //    string elmSourceString = String.Join("\n    ,", elmSources);

        //    return ",  sources = [" + elmSourceString + "] ";
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

            var conflicts = new List<Conflict>();
            ImportPage(conflicts, path + "\\" + "President Donald J. Trump and FLOTUS.html");
            ImportPage(conflicts, path + "\\" + "Donald Trump Jr., Eric Trump, Ivanka Trump & Jared Kushner.html");

            WriteJson(conflicts, path + "\\" + "conflicts.json"); 
            Console.WriteLine(conflicts.Count.ToString() + " total conflicts");
        }


        private void WriteJson(List<Conflict> conflicts, string file) {
            var strings = new StringBuilder();

            foreach (Conflict conflict in conflicts)
                strings.Append(conflict.ToJson() + "\n");

            System.IO.File.WriteAllText(file, strings.ToString());
        }
        


        private void ImportPage(List<Conflict> conflicts, string file) {
            
            string text = File.ReadAllText(file, Encoding.UTF8);

            int count = 0;
            int max = 10;
            var rows = text.Split(new[] { "<tr " }, StringSplitOptions.None);
            foreach (string row in rows) {
                if (count < max) {
                    AddConflict(conflicts, row);
                    count++;
                }
            }
            Console.WriteLine(conflicts.Count.ToString() + " conflicts");
        }

        private void AddConflict(List<Conflict> conflicts, string row) {
            var cols = row.Split(new[] { "<td " }, StringSplitOptions.None);

            if (cols.Length < 2)
                return;

            if (cols[1].Contains("></td>")) 
                return;

            if (cols[1].Contains("Description</td"))
                return;

            conflicts.Add(new Conflict(
                Description(cols[1])
            ));
                
        }

        private string Description(string txt) {

            var subs = txt.Split('>');
            return 
                subs[1].Replace("</td", "").TrimEnd().TrimStart();

        }
    }
}
