using System.IO;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public static class AppFiles
    {
        /// <summary>
        /// json 文件夹（当前程序目录下）
        /// </summary>
        public static string JsonDir =>
            Path.Combine(
                Application.StartupPath,
                "json"
            );

        /// <summary>
        /// 条码规则文件
        /// </summary>
        public static string BarcodeRuleJson =>
            Path.Combine(
                JsonDir,
                "BarcodeRule.json"
            );

        /// <summary>
        /// 单据类型选择配置（Form3）
        /// </summary>
        public static string BillTypeSelectJson =>
            Path.Combine(
                JsonDir,
                "BillTypeSelect.json"
            );
    }
}
