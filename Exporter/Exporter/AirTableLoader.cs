using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;


namespace Phase2 {

    public class AirTableLoader {

        public static void Load(string path) {
            LoadConflicts(path + "Conflicts-Grid view.csv");
            LoadStories(path + "Stories-Grid view.csv");

        }


        private static void LoadConflicts(string file) {



            var csv = GetCsvParser(file);
            csv.ReadLine();

            int conflicts = 0;
            while (!csv.EndOfData) {
                string[] fields = csv.ReadFields();
                string Name = fields[0];
                
                conflicts++;
            }
            Console.WriteLine("Conflicts: " + conflicts.ToString());
        }


        private static void LoadStories(string file) {
            var csv = GetCsvParser(file);
            csv.ReadLine();

            int stories = 0;
            while (!csv.EndOfData) {
                string[] fields = csv.ReadFields();
                string Name = fields[0];
                
                stories++;
            }
            Console.WriteLine("Stories: " + stories.ToString());
        }

        private static TextFieldParser GetCsvParser(string file) {
            TextFieldParser csv = new TextFieldParser(file);
            csv.CommentTokens = new string[] { "#" };
            csv.SetDelimiters(new string[] { "," });
            csv.HasFieldsEnclosedInQuotes = true;

            return csv;
        }
    }
}
