using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Conflicts {
    
    class Program {

        static void Main(string[] args) {
            //SpreadsheetGear.IWorkbook workbook = SpreadsheetGear.Factory.GetWorkbook("c:\\trump-conflicts\\Exporter\\Exporter\\data\\trump-conflicts6.xlsx");
            //SpreadsheetGear.IWorksheet worksheet = workbook.Worksheets["trump-conflicts"];
            //var h = worksheet.Hyperlinks;
            //SpreadsheetGear.IRange cells = worksheet.Cells;

            //var loader = new ConflictLoader(cells);


            var loader = new HtmlLoader("c:\\trump-conflicts\\Exporter\\Exporter\\data\\Tracking Trump's Conflicts of Interest");

            Console.ReadLine();
        }
    }
}
