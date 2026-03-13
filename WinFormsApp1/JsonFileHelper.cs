using System.IO;
using System.Text.Json;

public static class JsonFileHelper
{
    /// <summary>
    /// 读取 JSON 文件并反序列化
    /// </summary>
    public static T Load<T>(string filePath) where T : new()
    {
        if (!File.Exists(filePath))
            return new T();

        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // ⭐ 允许忽略属性名大小写
        }) ?? new T();
    }

    /// <summary>
    /// 保存对象到 JSON 文件
    /// </summary>
    public static void Save<T>(string filePath, T data)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions
            {
                WriteIndented = true
            }
        );

        File.WriteAllText(filePath, json);
    }
}
