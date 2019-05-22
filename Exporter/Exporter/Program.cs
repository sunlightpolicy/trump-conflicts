using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;



namespace Phase2 {
    
        public class Program {

            public static string ImportDate = "November 29 2018";
            public static string inputPath = "c:\\trump-conflicts\\Exporter\\Exporter\\data\\airtable\\";
            public static string outputPath = "c:\\trump-conflicts\\Exporter\\Exporter\\db\\Versions\\Updates\\";


        static void Main(string[] args) {


            //AirTableLoader.LoadConflictsAndStories(inputPath, outputPath);

            // Run scripts generated above before running this
            //AirTableLoader.WriteSlugs(outputPath);
                
            //AirTableLoader.LoadEthics("c:\\trump-conflicts\\Exporter\\Exporter\\data\\airtable\\", "C:\\trump-conflicts\\Exporter\\Exporter\\db\\Versions\\Updates\\Ethics\\");
           
            // Queries db and generates json - need to run scripts above before running this
            JsonGenerator.RunConflicts("c:\\trump-conflicts\\data\\");
                        
            Console.WriteLine("Done");
            Console.ReadLine();

                // Timelines aren't used at the moment 
                //TimelineImport.MakeJson("c:\\trump-conflicts\\data\\media\\");
                //TimelineJsImport.MakeJson("c:\\trump-conflicts\\data\\timeline\\");
         }
    }
}
