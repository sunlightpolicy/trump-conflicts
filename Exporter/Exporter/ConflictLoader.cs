using System;
using System.Collections.Generic;
using System.Text;

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

        public string Elm() {
            return "{ " +
                 "name = \"" + Util.RemoveQuotes(Name) + "\",  " +
                 "link = \"" + Util.RemoveQuotes(Link) + "\",  " +
                 "date = \"" + Date.ToString("d") + "\" " +
                 "}";
        }
    }


    public class Conflict {

        private SpreadsheetGear.IRange cells;

        public string Description;
        public string FamilyMember;
        public string ConflictingEntity;
        public string Category;
        public string Notes;
        public DateTime DateAddedOrEdited;
        public List<Source> Sources;

        public Conflict(SpreadsheetGear.IRange cells, int row) {
            this.cells = cells;

            Description = (string)cells[row, 0].Value;
            FamilyMember = (string)cells[row, 1].Value;
            ConflictingEntity = (string)cells[row, 2].Value;
            Category = (string)cells[row, 9].Value;
            Notes = (string)cells[row, 10].Value;
            DateAddedOrEdited = cells.Worksheet.Workbook.NumberToDateTime((double)cells[row, 11].Value);

            Sources = new List<Source>();
            AddSource(row, 3);
            AddSource(row, 5);
            AddSource(row, 7);
        }

        private void AddSource(int row, int col) {
            string source = "";
            string link = "";
            DateTime date = DateTime.Now;
            if ((cells[row, col].Value != null) || (cells[row, col + 1].Value != null)) {
                bool valid = true;

                try {  // Test for valid source
                    source = (String)cells[row, col].Value;
                    link = GetLink((String)cells[row, col].Formula);
                }
                catch {
                    valid = false;
                    Console.WriteLine("Source at row " + Convert.ToString(row) + ", column " + Convert.ToString(col) +
                        " not valid");
                }

                try {  // Test for valid date
                    date = cells.Worksheet.Workbook.NumberToDateTime((double)cells[row, col + 1].Value);
                }
                catch {
                    valid = false;
                    Console.WriteLine("Date at row " + Convert.ToString(row) + ", column " + Convert.ToString(col + 1) +
                        " not valid (" + (string)cells[row, col + 1].Value + ")");
                }

                if (valid)
                    Sources.Add(new Source(source, link, date));
            }
        }

        public string Elm() {
            return
                "{" + ElmFields() + ElmSources() + "}  ";
        }

        private string GetLink(string txt) {
            //  = HYPERLINK("https://www.wsj.com/articles/trump-debts-are-widely-held-on-wall-street-creating-new-potential-conflicts-1483637414", "Wall Street Journal")
            string link = txt; 

            int first = link.IndexOf("http");
            link = link.Substring(first, txt.Length - first);
             
            int last = link.IndexOf("\",");
            link = link.Substring(0, last);

            return link;
        }

        private string ElmFields() {
            return 
                 "description = \"" + Util.RemoveQuotes(Description) + "\", " +
                 "familyMember = \"" + TrumpName(Util.RemoveQuotes(FamilyMember)) + "\", " +
                 "conflictingEntity = \"" + Util.RemoveQuotes(ConflictingEntity) + "\", " +
                 "category = \"" + UpperCaseFirstChar(Util.RemoveQuotes(Category)) + "\", " +
                 "notes = \"" + Util.RemoveQuotes(Notes) + "\", " +
                 "dateAddedOrEdited = \"" + DateAddedOrEdited.ToString("d") + "\"  "; ;
        }

        private string ElmSources() {
            List<String> elmSources = new List<String>();
            foreach (Source source in Sources)
                elmSources.Add(source.Elm());
            string elmSourceString = String.Join("\n    ,", elmSources);
            
            return ",  sources = [" + elmSourceString + "] ";
        }

        private string UpperCaseFirstChar(string str) {
            if (string.IsNullOrEmpty(str)) {
                return string.Empty;
            }return char.ToUpper(str[0]) + str.Substring(1);
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

    public class ConflictLoader {

        private string outputFile = "c:\\trump-conflicts\\Exporter\\Exporter\\data\\Data.elm";
        private StringBuilder strings;
        SpreadsheetGear.IRange cells;

        public List<Conflict> Conflicts { get; set; }

        public ConflictLoader(SpreadsheetGear.IRange cells) {
            this.cells = cells;

            GetConflicts();
            MakeElm();
        }

        private void GetConflicts() {
            Conflicts = new List<Conflict>();

            int row = 1;
            bool more = true;
            while (more) {
                if (cells[row, 0].Value == null) {
                    more = false;
                    break;
                }
                Conflicts.Add(new Conflict(cells, row));
                row++;
            }
            Console.WriteLine(Convert.ToString(Conflicts.Count + " conflicts read"));
        }

        private void MakeElm() {
            strings = new StringBuilder();

            WriteHeader();
            WriteConflicts();

            System.IO.File.WriteAllText(outputFile, strings.ToString());
        }

        private void WriteHeader() {
            Line("module Data exposing (conflictList)");
            Line("");
            Line("import Types exposing (..)");
            Line("");
            Line("");
        }

        private void WriteConflicts() {
            List<String> elmConflicts = new List<String>();
            foreach (Conflict conflict in Conflicts)
                elmConflicts.Add(conflict.Elm());
            string elmConflictsString = String.Join("\n    ,", elmConflicts);
            
            Line("conflictList : List Conflict");
            Line("conflictList = [");
            Line("    " + elmConflictsString);
            Line("    ]");
        }

        private void Line(String str) {
            strings.Append(str + "\n");
        }        
    }

    public class Util {
        public static string RemoveQuotes(String str) {
            if (str == null)
                return "";
            return str.Replace("\"", "");
        }
    }
}
