using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;



namespace Phase2 {
    
        public class Program {

            public static string ImportDate = "September 17 2018";

            static void Main(string[] args) {

                //AirTableLoader.LoadConflictsAndStories("c:\\trump-conflicts\\Exporter\\Exporter\\data\\airtable\\", "C:\\trump-conflicts\\Exporter\\Exporter\\db\\Versions\\Updates\\");
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
