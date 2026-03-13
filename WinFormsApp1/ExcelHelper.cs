using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel; // Excel 97-2003
using NPOI.SS.UserModel;   // 通用接口
using NPOI.XSSF.UserModel; // Excel 2007+

namespace WinFormsApp1
{
    public class ExcelHelper
    {
        /// <summary>
        /// 读取 Excel 文件的第一个 Sheet 到 DataTable
        /// </summary>
        public static DataTable ImportExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = null;

                // 根据扩展名选择 Workbook
                if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    throw new Exception("不支持的文件格式，仅支持 .xlsx 和 .xls");
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                {
                    throw new Exception("Excel 文件为空或无法读取");
                }

                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    throw new Exception("Sheet1 为空");
                }

                // 获取表头行 (假设第一行是表头)
                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                {
                    throw new Exception("第一行为空，请确保包含表头");
                }

                int cellCount = headerRow.LastCellNum;

                // 创建列
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    ICell cell = headerRow.GetCell(i);
                    string colName = cell?.ToString() ?? $"Column{i}";

                    if (dt.Columns.Contains(colName))
                    {
                        colName = colName + "_" + i; // 防止重名
                    }
                    dt.Columns.Add(colName);
                }

                // 读取数据
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    DataRow dataRow = dt.NewRow();
                    bool hasValue = false;

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (j < 0) continue; // 防御

                        ICell cell = row.GetCell(j);
                        if (cell != null)
                        {
                            // 根据类型读取值
                            // 简单起见，全部转为 String，或者 NPOI 会自动处理 ToString()
                            // 如果是公式，可能需要求值
                            if (cell.CellType == CellType.Formula)
                            {
                                try
                                {
                                    // 尝试获取计算后的值
                                    dataRow[j] = cell.NumericCellValue;
                                    hasValue = true;
                                }
                                catch
                                {
                                    dataRow[j] = cell.ToString();
                                    hasValue = true;
                                }
                            }
                            else
                            {
                                string val = cell.ToString();
                                dataRow[j] = val;
                                if (!string.IsNullOrWhiteSpace(val)) hasValue = true;
                            }
                        }
                    }

                    // 只有当行内有数据时才添加
                    if (hasValue)
                    {
                        dt.Rows.Add(dataRow);
                    }
                }
            }

            return dt;
        }
    }
}
