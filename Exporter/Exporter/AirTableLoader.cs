using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;


namespace Phase2 {

    

    public class Column {
        public int SourceIndex;    // column in csv
        public string Name;        // Name in target db table
        public string LookupTable; // Optional table for foriegn key (use Name)  
        public Column(int sourceIndex, string name, string lookupTable = "") {
            SourceIndex = sourceIndex;
            Name = name;
            LookupTable = lookupTable;
        }
    }

    public class Table {

        public enum Crud { Insert, Uplate }

        public string TableName; // Name of db table
        public string CsvFile;   // Full path to csv file
        public List<Column> Columns;
        public string KeyField;  // Unique nme field of db table
        
        public Table(string tableName, string csvFile, List<Column> columns, string keyField = "Name") {
            TableName = tableName;
            CsvFile = csvFile;
            Columns = columns;
            KeyField = keyField;
        }


        // Makes INSERT or UPDATE for each row
        public string Load() {

            var csv = GetCsvParser(CsvFile);

            // Ignore header rows
            csv.ReadLine();

            // Made dictionary of keys
            // "SELECT * FROM " + tableName + " WHERE " + keyField + " = X
            
            int conflicts = 0;
            while (!csv.EndOfData) {
                string[] fields = csv.ReadFields();
                

                conflicts++;
            }
            Console.WriteLine("Conflicts: " + conflicts.ToString());
            return "";
        }


        // Returns an INSERT OR AN UPDATE
        public string Sql(string[] fields, Crud crudOp) {

            if (crudOp == Crud.Insert) {

            } else {

            }
            return "";
        }

        private static TextFieldParser GetCsvParser(string csvFile) {
            TextFieldParser csv = new TextFieldParser(csvFile);
            csv.CommentTokens = new string[] { "#" };
            csv.SetDelimiters(new string[] { "," });
            csv.HasFieldsEnclosedInQuotes = true;

            return csv;
        }
    }


    public class AirTableLoader {

        public static void Load(string path) {
            LoadConflicts(path + "Conflicts-Grid view.csv");
            LoadStories(path + "Stories-Grid view.csv");
        }
        
        private static void LoadConflicts(string csvFile) {

            Table conflictTable = new Table(
                "Conflict",
                csvFile,
                new List<Column>() {
                    new Column(0, "Name"),
                    new Column(2, "Description"),
                    new Column(3, "Notes"),
                    new Column(5, "ConflictPublicationStatusID", "ConflictPublicationStatus"),
                    new Column(6, "InternalNotes"),
                }
            );
            conflictTable.Load();
        }
        
        private static void LoadStories(string csvFile) {

            Table storyTable = new Table(
                "Story",
                csvFile,
                new List<Column>() {
                    new Column(0, "Link"), // URL of Story
                    new Column(1, "Headline"), // Headline
                    new Column(2, "MediaOutletID", "MediaOutlet"), // Media Outlet
                    new Column(3, "Date"), // Publication Date
                    new Column(4, ""), // Conflicts!!!
                    new Column(5, "EditorID", "SystemUser"), // EnteredBy
                    new Column(6, "StoryStatusID", "StoryStatus"), // Pub Status
                    new Column(7, "InternalNotes"), // Internal Notes
                    new Column(8, "Notes") // Notes
                }
            );
            storyTable.Load();
        }
    }
}
