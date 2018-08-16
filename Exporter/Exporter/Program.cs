using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;



namespace Phase2 {
    
        class Program {

        static void Main(string[] args) {

            //var loader = new ConflictLoader(cells);
            // var loader = new HtmlLoader("c:\\trump-conflicts\\Exporter\\Exporter\\data\\Tracking Trump's Conflicts of Interest");


            // Reads the google doc and generates sql inserts (which need to be run)
            //var loader = new Phase2Loader("c:\\trump-conflicts\\Exporter\\Exporter\\data\\WORKING CURRENT COPY Tracking Trump's Conflicts of Interest");




            //AirTableLoader.Load("c:\\trump-conflicts\\Exporter\\Exporter\\data\\airtable\\", "C:\\trump-conflicts\\Exporter\\Exporter\\db\\Versions\\Updates\\");
            //AirTableLoader.LoadEthics("c:\\trump-conflicts\\Exporter\\Exporter\\data\\airtable\\", "C:\\trump-conflicts\\Exporter\\Exporter\\db\\Versions\\Updates\\Ethics\\");


            // Queries db and generates json - need to run scripts above before running this
            // NOT USED
            //JsonGenerator.Run("c:\\trump-conflicts\\data\\");

            JsonGenerator.RunConflicts("c:\\trump-conflicts\\data\\");


            // Write script to update sslugs
            //AirTableLoader.WriteSlugs("C:\\trump-conflicts\\Exporter\\Exporter\\db\\Versions\\Updates\\");

            // Timeline JS
            //TimelineJsImport.MakeJson("c:\\trump-conflicts\\data\\timeline\\");

            Console.WriteLine("Done");
            Console.ReadLine();


                // NOT USED
                //TimelineImport.MakeJson("c:\\trump-conflicts\\data\\media\\");
         }
    }
}
