namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            // ⭐ 1. 读取配置文件
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var config = JsonFileHelper.Load<AppConfig>(configPath);



            if (string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                // 如果没读到，写入默认值方便用户修改
                config.ConnectionString = "Server=172.16.1.9;Database=UFDATA_008_2021;User Id=sa;Password=Ks123456;TrustServerCertificate=True;";
                JsonFileHelper.Save(configPath, config);
            }

            SqlServerDb.ConnectionString = config.ConnectionString;

            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form4());
        }
    }
}