using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conflicts {
    
    class Program {

        static void Main(string[] args) {
            SpreadsheetGear.IWorkbook workbook = SpreadsheetGear.Factory.GetWorkbook("c:\\trump-conflicts\\Exporter\\Exporter\\data\\trump-conflicts2.xlsx");
            SpreadsheetGear.IWorksheet worksheet = workbook.Worksheets["trump-conflicts"];

            var h = worksheet.Hyperlinks;

            SpreadsheetGear.IRange cells = worksheet.Cells;

            var loader = new ConflictLoader(cells);
            Console.ReadLine();
        }
    }
}
