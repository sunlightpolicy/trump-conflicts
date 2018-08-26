using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Phase2 {

    public enum ImportType { Insert, InsertUpdate, Reload }


    public class Column {
        public int SourceIndex;         // Column in csv
        public string Name;             // Name in target db table
        public string LookupTable;      // Optional table for foriegn key (use Name)  
        public string LookupNameField;  // Field to match in lookup table. Default to "Name", but could be something else
        public string DefaultValue;     // Use this if no value is specified
        public Column(int sourceIndex, string name, string lookupTable = "", string lookupNameField = "Name", string defaultValue = "") {
            SourceIndex = sourceIndex;
            Name = name;
            LookupTable = lookupTable;
            LookupNameField = lookupNameField;
            DefaultValue = defaultValue;
        }
    }

    public class Table {

        public enum CrudOp { Insert, Update }

        
        public string FileNumber;   // First part of file name 
        public string TableName;    // Name of db table
        public string CsvFile;      // Full path to csv file
        public string OutputPath;   // Where sql file goes
        public List<Column> Columns;
        public string KeyField;     // Unique name field of db table

        public Table(string fileNumber, string tableName, string csvFile, string outputPath, List<Column> columns, string keyField = "Name") {
            FileNumber = fileNumber;
            TableName = tableName;
            CsvFile = csvFile;
            OutputPath = outputPath;
            Columns = columns;
            KeyField = keyField;
        }


        // Makes INSERT or UPDATE for each row and write to file
        public virtual void Load() {

            List<string> cmds = new List<string>();
            if (TableName != "StoryConflict")
                cmds = LoadCsv();
            else
                cmds = LoadConflictStatus();


            using (TextWriter tw = new StreamWriter(this.OutputPath + this.FileNumber + " " + this.TableName + ".sql")) {
                tw.WriteLine("USE Trump");
                tw.WriteLine("GO");

                if (TableName == "StoryConflict") {
                    tw.WriteLine("DELETE FROM StoryConflict");
                }

                if (TableName == "Story") {
                    tw.WriteLine("DELETE FROM StoryConflict");
                    tw.WriteLine("GO");
                    //tw.WriteLine("DELETE FROM Story");
                    ///tw.WriteLine("GO");
                }

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
                //string[] conflicts = fields[4].Split(',');
                List<string> conflicts = GetColumns(fields[4]);
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



        protected virtual List<string> LoadCsv() {
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

            // Key field is blank
            if (fields[Columns[0].SourceIndex] == "")
                return "";

            for (int i = 0; i < this.Columns.Count; i++) {
                cols.Add(Columns[i].Name);

                // Could be a column-specific default, but they are now all blank strings
                string val = "";
                if (Columns[i].SourceIndex != -1)
                    val = fields[Columns[i].SourceIndex].Replace("\"", "").Replace("'", "''");

                vals.Add(ValueSql(Columns[i], val));
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


        // Parse a csv string. It can handle some of the fields being quoted (because they have commas) and some not quoted
        private List<string> GetColumns(string input) {
            var str = input;

            // No quotes, just return comma separated items 
            if (!str.Contains('\"'))
                return str.Split(',').ToList();

            List<string> list = new List<string>();
            if (string.IsNullOrWhiteSpace(str))
                return list;

            int firstQ = -1;
            bool more = true;
            while (more) {
                for (int i = 0; i < str.Length; i++) {
                    if (str[i] == '\"') {
                        if (firstQ == -1) {
                            firstQ = i;
                        }
                        else {
                            list.Add(str.Substring(firstQ + 1, i - firstQ - 1));
                            str = str.Remove(firstQ, i - firstQ + 2);
                            firstQ = -1;
                            break;
                        }
                    }
                    if (i == str.Length - 1)
                        more = false;
                }
            }
            var others = str.Split(',').ToList();
            foreach (string s in others)
                list.Add(s);

            return list;
        }
    }


    public class EthicsTable: Table {

        public ImportType ImportType;
        public bool Delete;

        public EthicsTable(string fileNumber, string tableName, string csvFile, string outputPath, List<Column> columns, string keyField = "Name", ImportType importType = ImportType.Reload, bool delete = false) :
            base(fileNumber, tableName, csvFile, outputPath, columns, keyField) {
            ImportType = importType;
            Delete = delete;
        }


        protected override List<string> LoadCsv() {
            List<string> keys = null;
            if (ImportType == ImportType.InsertUpdate)
                keys = GetKeys();

            // Ignore header rows
            var csv = GetCsvParser(CsvFile);
            csv.ReadLine();

            var cmds = new List<string>();
            while (!csv.EndOfData) {
                string[] fields = csv.ReadFields();

                // If the conflict field is blank, ignore it.
                if ((fields[4].Trim() == "") && (TableName == "Conflict" || TableName == "BusinessConflict"))
                    continue;

                if ((fields[Columns[0].SourceIndex].Trim() == "") && (TableName == "BusinessOwnership"))
                    continue;

                //bool exists = false;
                //if (TableName == "Conflict")
                //    exists = keys.Contains(fields[4].ToLower());

                switch (ImportType) {
                    case ImportType.Insert:
                        cmds.Add(InsertStatment(fields));
                        break;
                    case ImportType.InsertUpdate:
                        if (keys.Contains(fields[4].ToLower())) // Conflict field
                            cmds.Add(InsertStatment(fields));
                        else
                            cmds.Add(UpdateStatment(fields));
                        break;
                    case ImportType.Reload:
                        cmds.Add(InsertStatment(fields));
                        break;
                }
            }
            return cmds;
        }


        public override void Load() {

            List<string> cmds = new List<string>();
            cmds = LoadCsv();
            
            using (TextWriter tw = new StreamWriter(this.OutputPath + this.FileNumber + " " + this.TableName + ".sql")) {
                tw.WriteLine("USE Trump");
                tw.WriteLine("GO");

                if (this.ImportType == ImportType.Reload && Delete) {
                    tw.WriteLine("DELETE FROM " + TableName);
                    tw.WriteLine("GO");
                }

                foreach (String cmd in cmds)
                    tw.WriteLine(cmd);
            }
        }
    }


    public class AirTableLoader {

        public static void LoadConflictsAndStories(string inputPath, string outputPath) {
            LoadConflicts(inputPath + "Conflicts-Grid view.csv", outputPath);
            LoadStories(inputPath + "Stories-Grid view.csv", outputPath);
            LoadStoryConflicts(inputPath + "Stories-Grid view.csv", outputPath);

            WriteSlugs(outputPath);
        }

        public static void WriteSlugs(string outputPath) {

            var slugUpdates = new List<string>();

            string query = "SELECT ID, Name FROM Conflict";
            using (SqlConnection conn = new SqlConnection("Server=SCOTT-PC\\SQLExpress;Database=Trump;Trusted_Connection=True;")) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        slugUpdates.Add("UPDATE Conflict SET Slug = '" + ToUrlSlug(reader["Name"].ToString()) + "' WHERE ID = " + reader["ID"].ToString());
                }
            }
            using (TextWriter tw = new StreamWriter(outputPath + "04 UpdateSlugs.sql")) {
                tw.WriteLine("USE Trump");
                tw.WriteLine("GO");

                foreach (String cmd in slugUpdates)
                    tw.WriteLine(cmd);
            }
        }

        public static string ToUrlSlug(string value) {

            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }


        private static void LoadConflicts(string csvFile, string outputPath) {

            Table conflictTable = new Table(
                "01",
                "Conflict",
                csvFile,
                outputPath,
                new List<Column>() {
                    new Column(0, "Name"),
                    new Column(1, "ConflictStatusID", "ConflictStatus"),
                    new Column(2, "Description"),
                    new Column(3, "Notes"),
                    new Column(4, "ConflictPublicationStatusID", "ConflictPublicationStatus"),
                    new Column(5, "InternalNotes"),
                }
            );
            conflictTable.Load();
        }
        
        private static void LoadStories(string csvFile, string outputPath) {

            Table storyTable = new Table(
                "02",
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
                "03",
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


        public static void LoadEthics(string inputPath, string outputPath) {
            LoadEthicsConflicts(inputPath + "Ethics Doc Lines-Grid view.csv", outputPath);
            LoadEthicsBusiness(inputPath + "Ethics Doc Lines-Grid view.csv", outputPath);
            LoadEthicsBusinessConflicts(inputPath + "Ethics Doc Lines-Grid view.csv", outputPath);
            LoadEthicsFamilyMemberBusiness(inputPath + "Ethics Doc Lines-Grid view.csv", outputPath);
            LoadEthicsBusinessOwnership(inputPath + "Ethics Doc Lines-Grid view.csv", outputPath);
        }


        private static void LoadEthicsConflicts(string csvFile, string outputPath) {

            EthicsTable table = new EthicsTable(
                "01",
                "Conflict",
                csvFile,
                outputPath,
                new List<Column>() {
                    new Column(4, "Name"),
                    new Column(-1, "Description"),
                    new Column(-1, "Notes"),
                    new Column(-1, "ConflictPublicationStatusID", "ConflictPublicationStatus"), // !
                    new Column(-1, "InternalNotes"),
                },
                "Name",
                ImportType.Insert
            );
            table.Load();
        }


        private static void LoadEthicsBusiness(string csvFile, string outputPath) {

            var colNums = new int[] { 5, 7, 9, 11, 13 };
            int count = 2;
            foreach (int col in colNums) {

                EthicsTable table = new EthicsTable(
                    "0" + count.ToString(),
                    "Business",
                    csvFile,
                    outputPath,
                    new List<Column>() {
                        new Column(col, "Name"),
                    },
                    "Name",
                    ImportType.Insert
                );
                table.Load();
                count++;
            }
        }


        private static void LoadEthicsBusinessConflicts(string csvFile, string outputPath) {

            EthicsTable table = new EthicsTable(
                "07",
                "BusinessConflict",
                csvFile,
                outputPath,
                new List<Column>() {
                    new Column(4, "ConflictID", "Conflict", "Name"),
                    new Column(5, "BusinessID", "Business", "Name"), 
                },
                "Name",
                ImportType.Reload
            );
            table.Load();
        }


        private static void LoadEthicsFamilyMemberBusiness(string csvFile, string outputPath) {

            EthicsTable table = new EthicsTable(
                "08",
                "FamilyMemberBusiness",
                csvFile,
                outputPath,
                new List<Column>() {
                    new Column(6, "FamilyMemberID", "FamilyMember", "Name"),
                    new Column(5, "BusinessID", "Business", "Name"),
                    new Column(16, "FamilyMemberConflictStatusID", "FamilyMemberConflictStatus", "Name"),
                    new Column(3, "Description")
                },
                "Name",
                ImportType.Reload
            );
            table.Load();
        }

        private static void LoadEthicsBusinessOwnership(string csvFile, string outputPath) {

            var colNums = new int[] { 7, 9, 11, 13 };
            int count = 9;
            foreach (int col in colNums) {

                EthicsTable table = new EthicsTable(
                    "0" + count.ToString(),
                    "BusinessOwnership",
                    csvFile,
                    outputPath,
                    new List<Column>() {
                         new Column(col, "OwnerID", "Business", "Name"),
                         new Column(5, "OwneeID", "Business", "Name"),
                         new Column(col + 1, "OwnershipPercentage")
                    },
                    "Name",
                    ImportType.Reload,
                    col == colNums[0]  // If it is the first of these, Delete the records 
                );
                table.Load();
                count++;
            }
        }
    }
}
