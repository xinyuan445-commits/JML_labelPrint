using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Launcher;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        // 确保显示出来
        this.Show();
        await Task.Delay(500);

        try
        {
            // 1. 读取配置
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                LaunchApp();
                return;
            }

            string jsonString = await File.ReadAllTextAsync(configPath);
            var jsonNode = JsonNode.Parse(jsonString);
            string remotePath = jsonNode?["RemotePath"]?.ToString() ?? "";
            string localExeName = jsonNode?["LocalExeName"]?.ToString() ?? "WinFormsApp1.exe";

            if (string.IsNullOrWhiteSpace(remotePath) || !Directory.Exists(remotePath))
            {
                // 配置无效或路径不可达，直接启动本地
                LaunchApp(localExeName);
                return;
            }

            string remoteExePath = Path.Combine(remotePath, localExeName);
            string localExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localExeName);

            if (!File.Exists(remoteExePath))
            {
                LaunchApp(localExeName);
                return;
            }

            // 2. 比较版本
            bool needUpdate = false;
            if (!File.Exists(localExePath))
            {
                needUpdate = true;
            }
            else
            {
                var localVer = FileVersionInfo.GetVersionInfo(localExePath).FileVersion;
                var remoteVer = FileVersionInfo.GetVersionInfo(remoteExePath).FileVersion;

                // 简单字符串比较，或者 Version.Parse
                if (localVer != remoteVer)
                {
                    // 只要版本不同就更新（通常远程是新的）
                    // 也可以 Version.Parse(remoteVer) > Version.Parse(localVer)
                    needUpdate = true;
                }
            }

            if (needUpdate)
            {
                label1.Text = "发现新版本，正在更新...";
                
                // 关闭主程序
                var processName = Path.GetFileNameWithoutExtension(localExeName);
                var processes = Process.GetProcessesByName(processName);
                foreach (var p in processes)
                {
                    try { p.Kill(); p.WaitForExit(); } catch { }
                }

                // 复制文件
                await Task.Run(() => CopyDirectory(remotePath, AppDomain.CurrentDomain.BaseDirectory));
            }

            // 3. 启动
            LaunchApp(localExeName);
        }
        catch (Exception ex)
        {
            MessageBox.Show("启动失败：" + ex.Message);
            // 尝试启动本地
            LaunchApp();
        }
    }

    private void CopyDirectory(string sourceDir, string destDir)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists) return;

        // 1. 计算总文件数（为了计算进度）
        int totalFiles = CountFiles(dir);
        int copiedFiles = 0;

        Directory.CreateDirectory(destDir);

        // 递归复制
        CopyDirectoryRecursive(dir, destDir, totalFiles, ref copiedFiles);
    }

    private void CopyDirectoryRecursive(DirectoryInfo sourceDir, string destDir, int totalFiles, ref int copiedFiles)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in sourceDir.GetFiles())
        {
            if (IsExcluded(file.Name)) continue;

            string destPath = Path.Combine(destDir, file.Name);
            try
            {
                file.CopyTo(destPath, true);
            }
            catch { }

            // 更新进度
            copiedFiles++;
            UpdateProgress(copiedFiles, totalFiles);
        }

        foreach (var subDir in sourceDir.GetDirectories())
        {
            CopyDirectoryRecursive(subDir, Path.Combine(destDir, subDir.Name), totalFiles, ref copiedFiles);
        }
    }

    private int CountFiles(DirectoryInfo dir)
    {
        int count = 0;
        try
        {
            foreach (var file in dir.GetFiles())
            {
                if (!IsExcluded(file.Name)) count++;
            }
            foreach (var subDir in dir.GetDirectories())
            {
                count += CountFiles(subDir);
            }
        }
        catch { }
        return count;
    }

    private bool IsExcluded(string fileName)
    {
        return fileName.StartsWith("Launcher", StringComparison.OrdinalIgnoreCase) ||
               fileName.Equals("appsettings.json", StringComparison.OrdinalIgnoreCase);
    }

    private void UpdateProgress(int current, int total)
    {
        if (total <= 0) return;
        int percent = (int)((double)current / total * 100);
        
        // 确保 UI 线程更新
        this.Invoke(new Action(() =>
        {
            progressBar1.Value = Math.Min(percent, 100);
            labelPercent.Text = $"{percent}%";
            label1.Text = $"正在更新 ({current}/{total})...";
        }));
    }

    private void LaunchApp(string exeName = "WinFormsApp1.exe")
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exeName);
        if (File.Exists(path))
        {
            Process.Start(path);
        }
        else
        {
            MessageBox.Show("找不到主程序：" + exeName);
        }
        Application.Exit();
    }
}
