using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonExcel
{
    public class ExcelWriter
    {
        public static void SaveReportTo<T>(List<T> dto, string fileName)
        {
            if (!fileName.Contains("xlsx") && !fileName.Contains("xls"))
                fileName += ".xlsx";
            using (var stream = CreateExcelAsStream(dto))
            {
                var fileStream = File.Create(fileName);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                fileStream.Close();
            }
        }

        public static MemoryStream CreateExcelAsStream<T>(List<T> dto)
        {
            var outputStream = new MemoryStream();
            var excel = CreateExcel(dto);
            excel.SaveAs(outputStream);
            return outputStream;
        }

        public static ExcelPackage CreateExcel<T>(List<T> rows)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackage = new ExcelPackage();


            excelPackage.Workbook.Properties.Author = "Sluchaj Global Assemblies";
            excelPackage.Workbook.Properties.Title = "Отчёт";

            //Create a sheet
            excelPackage.Workbook.Worksheets.Add("List 1");
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[0];
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            var headerIndex = 1;
            var headers = new Dictionary<PropertyInfo, string>();
            foreach (var property in typeof(T).GetProperties().Where(prop => prop.IsDefined(typeof(DisplayAttribute), false)))
            {
                foreach (object attr in property.GetCustomAttributes(true))
                {
                    if (attr is DisplayAttribute)
                    {
                        var authAttr = attr as DisplayAttribute;
                        headers.Add(property, authAttr?.Name ?? string.Empty);
                        var cell = ws.Cells[1, headerIndex];
                        cell.Value = authAttr?.Name ?? string.Empty;
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.Silver);
                        cell.AutoFitColumns(5);
                        headerIndex++;
                        break;
                    }
                }
            }
            int rowIndex = 2;
            foreach (var item in rows)
            {
                headerIndex = 1;
                foreach (var header in headers)
                {
                    ws.Cells[rowIndex, headerIndex].Value = header.Key.GetValue(item);
                    headerIndex++;
                }
                rowIndex++;
            }
            return excelPackage;
        }
    }
}
