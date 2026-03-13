using LabelManager2;
using System.Runtime.InteropServices;
// ⭐ 关键：给 CodeSoft 的 Application 起别名
using CSApp = LabelManager2.Application;

namespace WinFormsApp1.Services
{
    public class CodeSoftLabelPrintService
    {
        public void Print(
            string labPath,
            string printerName, // 新增参数
            string documentNo,
            string itemCode,
            string itemName,
            string batch,
            decimal qty,
            string sn,
            string traceBackCode,
            int copies
        )
        {
            CSApp lbl = null;
            Document doc = null;

            try
            {
                lbl = new CSApp();
                // 打开模板
                lbl.Documents.Open(labPath, false);
                doc = lbl.ActiveDocument;

                // 设置打印机
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    // 尝试使用 SwitchTo 方法切换打印机
                    // 注意：CodeSoft 不同版本接口可能不同，常见有 doc.Printer.SwitchTo() 或 doc.Printer.Name = ...
                    // 如果 doc.Printer.Name 无效，请尝试 doc.Printer.SwitchTo(printerName);
                    // 或者 app.PrinterSystem().Printers(printerName).MakeActive();

                    try
                    {
                        doc.Printer.SwitchTo(printerName);
                    }
                    catch
                    {
                        // 降级尝试
                        doc.Printer.Name = printerName;
                    }
                }

                // 变量赋值
                SetVariable(doc, "账套名", "科米龙");
                SetVariable(doc, "单据编码", documentNo);
                SetVariable(doc, "物料编码", itemCode);
                SetVariable(doc, "物料名称", itemName);
                SetVariable(doc, "批号", batch);
                SetVariable(doc, "数量", qty.ToString());
                SetVariable(doc, "sn", sn);
                SetVariable(doc, "条码", traceBackCode);

                // 打印
                doc.PrintDocument(copies);
            }
            catch (Exception ex)
            {
                // 可以在这里添加日志记录
                // Log.Error("打印失败", ex);
                throw new Exception($"打印服务异常: {ex.Message}", ex);
            }
            finally
            {
                // 1. 优先关闭并释放 Document 对象
                if (doc != null)
                {
                    try
                    {
                        // 尝试关闭文档，不保存更改
                        doc.Close(false);
                    }
                    catch { /* 忽略关闭时的次级异常 */ }

                    try
                    {
                        Marshal.ReleaseComObject(doc);
                    }
                    catch { /* 忽略释放时的异常 */ }

                    doc = null;
                }

                // 2. 退出并释放 Application 对象
                if (lbl != null)
                {
                    try
                    {
                        lbl.Quit();
                    }
                    catch { /* 忽略退出时的次级异常 */ }

                    try
                    {
                        Marshal.ReleaseComObject(lbl);
                    }
                    catch { /* 忽略释放时的异常 */ }

                    lbl = null;
                }
            }
        }

        /// <summary>
        /// 安全地设置变量值，防止某些变量不存在导致整个流程崩溃
        /// </summary>
        private void SetVariable(Document doc, string name, string value)
        {
            try
            {
                // 注意：这里仍然存在中间对象未释放的风险，但在方法级调用中通常可由 GC 最终回收
                // 为了极致性能，可以将 FormVariables 提取为对象并释放，但代码复杂度会剧增
                doc.Variables.FormVariables.Item(name).Value = value;
            }
            catch
            {
                // 如果模板中没有这个变量，直接忽略，不要阻断打印流程
            }
        }
    }
}
