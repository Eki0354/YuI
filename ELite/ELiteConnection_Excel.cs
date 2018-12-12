using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using System.Data.SQLite;

namespace ELite
{
    public partial class ELiteConnection
    {
        public void TransferFromExcel(int year, string path)
        {
            Application app = new Application();
            Workbooks wbks = app.Workbooks;
            _Workbook _wbk = wbks.Add(path);
            SQLiteTransaction tran = BeginTransaction();
            foreach(Worksheet sheet in _wbk.Sheets)
            {
                if (sheet.Name == "房态表") continue;
                int month = Convert.ToInt32(sheet.Name.Substring(0, sheet.Name.IndexOf("月")));
                int startRowIndex = 3;
                int endRowIndex = sheet.UsedRange.Row;
                for(int rowIndex = startRowIndex; rowIndex < endRowIndex + 1; rowIndex++)
                {
                    string roomNumber = GetRoomNumber(sheet, rowIndex);
                    int startColumnIndex = 4;
                    int endColumnIndex = DateTime.DaysInMonth(year, month) + 4;
                    for(int columnIndex = startColumnIndex; columnIndex < endColumnIndex + 1; columnIndex++)
                    {
                        Console.WriteLine(month + "," + rowIndex + "," + columnIndex);
                        DateTime resDate = new DateTime(year, month, columnIndex - 3);
                        Range range = sheet.Cells[rowIndex, columnIndex];
                        SaveRoom(range, roomNumber, resDate);
                    }
                }
            }
            tran.Commit();
            _wbk.Close(null, null, null);
            wbks.Close();
            app.Quit();
        }

        private string GetRoomNumber(Worksheet sheet, int rowIndex)
        {
            string roomNumber = "42";
            string value = sheet.Cells[rowIndex, 4].Value;
            if (string.IsNullOrEmpty(value))
                value = sheet.Cells[rowIndex - 1, 4].Value;
            if (string.IsNullOrEmpty(value))
            {
                if (!value.Contains(" ")) return roomNumber;
                return value.Substring(0, value.IndexOf(" "));
            }
            else
            {
                return value.Substring(0, (value[3] == '-' ? 5 : 3));
            }
        }

        private void SaveRoom(Range range, string roomNumber, DateTime resDate)
        {
            string value = range.Value;
            string comment = range.Comment.Text();
            if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(comment)) return;
            int roomSate = GetRoomState(range.Interior.ColorIndex);

        }

        private int GetRoomState(int colorIndex)
        {
            switch(colorIndex)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                default:
                    return 42;
            }
        }
    }
}
