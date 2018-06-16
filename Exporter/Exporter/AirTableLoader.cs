using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

using System.Data;
using System.Data.SqlClient;


namespace Phase2 {

    

    public class Column {
        public int SourceIndex;         // Column in csv
        public string Name;             // Name in target db table
        public string LookupTable;      // Optional table for foriegn key (use Name)  
        public string LookupNameField;  // Field to match in lookup table. Default to "Name", but could be something else
        public Column(int sourceIndex, string name, string lookupTable = "", string lookupNameField = "Name") {
            SourceIndex = sourceIndex;
            Name = name;
            LookupTable = lookupTable;
            LookupNameField = lookupNameField;
        }
    }

    public class Table {

        public enum CrudOp { Insert, Update }

        public string TableName;    // Name of db table
        public string CsvFile;      // Full path to csv file
        public string OutputPath;   // Where sql file goes
        public List<Column> Columns;
        public string KeyField;     // Unique name field of db table
        
        public Table(string tableName, string csvFile, string outputPath, List<Column> columns, string keyField = "Name") {
            TableName = tableName;
            CsvFile = csvFile;
            OutputPath = outputPath;
            Columns = columns;
            KeyField = keyField;
        }


        // Makes INSERT or UPDATE for each row and write to file
        public void Load() {            

            List<string> cmds = new List<string>();
            if (TableName != "StoryConflict") 
                cmds = LoadCsv();
            else
                cmds = LoadConflictStatus();


            using (TextWriter tw = new StreamWriter(this.OutputPath + this.TableName + ".sql")) {
                if (TableName == "StoryConflict")
                    tw.WriteLine("DELETE FROM StoryConflict");
                if (TableName == "Story")
                    tw.WriteLine("DELETE FROM Story");

                foreach (String cmd in cmds)
                    tw.WriteLine(cmd);
            }
        }

        private List<string> LoadConflictStatus() {

            var csv = GetCsvParser(CsvFile);
            csv.ReadLine();

            var storyConflicts = new List<Tuple<string, string>>();
            while (!csv.EndOfData) {
                string[] fields = csv.ReadFields();

                var story = fields[0];
                string[] conflicts = fields[4].Split(',');
                foreach (string conflict in conflicts)
                    if (story != "")
                        storyConflicts.Add(Tuple.Create(story.Trim(), conflict.Trim()));
            }

            var cmds = new List<string>();
            foreach (Tuple<string, string> storyConfict in storyConflicts)
                cmds.Add(
                    "INSERT INTO StoryConflict VALUES (" +
                    "(SELECT ID FROM Story WHERE Link = '" + storyConfict.Item1 + "'), " +
                    "(SELECT ID FROM Conflict WHERE Name = '" + storyConfict.Item2 + "'))"
                );
            return cmds;
        }



        private List<string> LoadCsv() {
            var keys = GetKeys();

            // Ignore header rows
            var csv = GetCsvParser(CsvFile);
            csv.ReadLine();

            var cmds = new List<string>();
            while (!csv.EndOfData) {
                string[] fields = csv.ReadFields();

                // Thought it was already utf8, but whatever
                // https://stackoverflow.com/questions/10888040/how-to-convert-%C3%A2%E2%82%AC-to-apostrophe-in-c?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
                for (int i = 0; i < fields.Count(); i++) {
                    // Actually this doesn't work either - fixed in with SQL 

                    //fields[i] = fields[i].Replace("â€™", "'");
                    //var bytes = Encoding.Default.GetBytes(fields[i]);
                    //fields[i] = Encoding.UTF8.GetString(bytes);
                }

                if (keys.Contains(fields[0].ToLower()))
                    cmds.Add(UpdateStatment(fields));
                else
                    cmds.Add(InsertStatment(fields));
            }
            return cmds;
        }



        protected string InsertStatment(string[] fields) {            

            var cols = new List<string>();
            var vals = new List<string>();
            for (int i = 0; i < this.Columns.Count; i++) {
                cols.Add(Columns[i].Name);
                vals.Add(ValueSql(Columns[i], fields[Columns[i].SourceIndex]));
            }
            string sql = "INSERT INTO " + TableName + " (" + string.Join(", ", cols) + ") VALUES (" + string.Join(", ", vals) + ")";

            return sql;
        }

        protected string UpdateStatment(string[] fields) {
            
            var sqlCols = new List<string>();
            for (int i = 1; i < this.Columns.Count; i++)
                sqlCols.Add(Columns[i].Name + " = " + ValueSql(Columns[i], fields[Columns[i].SourceIndex]));

            string sql = "UPDATE " + TableName + " SET ";
            sql += string.Join(", ", sqlCols);
            sql += " WHERE " + this.KeyField + " = '" + fields[0] + "'";

            return sql;
        }


        protected string ValueSql(Column col, string value) {
            if (col.LookupTable == "")
                return "'" + value.Replace("'", "''") + "'";
            else
                return "(SELECT ID FROM " + col.LookupTable + " WHERE " + col.LookupNameField + " = '" + value + "')";
        }


        protected List<string> GetKeys() {
            var keys = new List<string>();

            string query = "SELECT " + KeyField + " FROM " + this.TableName;
            using (SqlConnection conn = new SqlConnection("Server=SCOTT-PC\\SQLExpress;Database=Trump;Trusted_Connection=True;")) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        keys.Add(reader[this.KeyField].ToString().ToLower());
                }
            }
            return keys;
        }

        protected static TextFieldParser GetCsvParser(string csvFile) {
            TextFieldParser csv = new TextFieldParser(csvFile, Encoding.Default);
            csv.CommentTokens = new string[] { "#" };
            csv.SetDelimiters(new string[] { "," });
            csv.HasFieldsEnclosedInQuotes = true;

            return csv;
        }
    }




    public class AirTableLoader {

        public static void Load(string inputPath, string outputPath) {
            LoadConflicts(inputPath + "Conflicts-Grid view.csv", outputPath);
            LoadStories(inputPath + "Stories-Grid view.csv", outputPath);

            LoadStoryConflicts(inputPath + "Stories-Grid view.csv", outputPath);
        }
        
        private static void LoadConflicts(string csvFile, string outputPath) {

            Table conflictTable = new Table(
                "Conflict",
                csvFile,
                outputPath,
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
        
        private static void LoadStories(string csvFile, string outputPath) {

            Table storyTable = new Table(
                "Story",
                csvFile,
                outputPath,
                new List<Column>() {
                    new Column(0, "Link"), // URL of Story
                    new Column(1, "Headline"), // Headline
                    new Column(2, "MediaOutletID", "MediaOutlet"), // Media Outlet
                    new Column(3, "Date"), // Publication Date
                    //new Column(4, ""), // Conflicts!!!
                    new Column(5, "EditorID", "SystemUserView", "UserName"), // EnteredBy
                    new Column(6, "StoryStatusID", "StoryStatus"), // Pub Status
                    new Column(7, "InternalNotes"), // Internal Notes
                    new Column(8, "Notes") // Notes
                },
                "Link"
            );
            storyTable.Load();
        }

        private static void LoadStoryConflicts(string csvFile, string outputPath) {

            Table storyTable = new Table(
                "StoryConflict",
                csvFile,
                outputPath,
                new List<Column>() {
                    new Column(0, "StoryID", "Story", "Link"),
                    new Column(4, ""), // Comma-separated conflict names!! 
                }
            );
            storyTable.Load();
        }
    }
}
