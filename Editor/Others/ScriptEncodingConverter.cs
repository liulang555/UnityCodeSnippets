using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptEncodingConverter : MonoBehaviour
{
    [MenuItem("Assets/ScriptToUTF-8", false, 2)]
    public static void ScriptEncoding()
    {
        string path = string.Empty;
        UnityEngine.Object selectedObject = Selection.activeObject;
        if (selectedObject != null)
        {
            path = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
        }
        if (Directory.Exists(path))
        {
            UnityEngine.Debug.LogError("当前选中的是文件夹: " + path);
            ConvertDirectoryToUTF8(path);
            return;
        }
        else if (File.Exists(path))
        {
            UnityEngine.Debug.Log("当前选中的是文件: " + path);
            ConvertFileToUTF8(path);
        }
        else
        {
            UnityEngine.Debug.LogError("没有选中文件或文件夹");
        }
    }
    /// <summary>
    /// 将指定文件夹及其子文件夹中的所有文件编码转换为UTF-8
    /// </summary>
    /// <param name="directoryPath">文件夹路径</param>
    /// <returns>转换的文件数量</returns>
    public static int ConvertDirectoryToUTF8(string directoryPath)
    {
        int convertedCount = 0;
        try
        {
            // 获取当前文件夹中的所有文件
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                // 只处理脚本文件和文本文件
                string extension = Path.GetExtension(file).ToLower();
                if (extension == ".cs" || extension == ".js" || extension == ".txt" || extension == ".json" || extension == ".xml")
                {
                    if (ConvertFileToUTF8(file))
                    {
                        convertedCount++;
                    }
                }
            }

            // 递归处理子文件夹
            string[] directories = Directory.GetDirectories(directoryPath);
            foreach (string directory in directories)
            {
                convertedCount += ConvertDirectoryToUTF8(directory);
            }

            Debug.Log($"文件夹 {directoryPath} 中共转换了 {convertedCount} 个文件");
            return convertedCount;
        }
        catch (Exception ex)
        {
            Debug.LogError($"转换文件夹失败: {directoryPath}, 错误: {ex.Message}");
            return convertedCount;
        }
    }
    /// <summary>
    /// 将指定文件的编码转换为UTF-8
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否成功转换</returns>
    public static bool ConvertFileToUTF8(string filePath)
    {
        try
        {
            // 检查文件是否需要转换
            if (IsNeedConvertToUtf8(filePath))
            {
                // 读取文件内容（使用GB2312编码）
                var text = File.ReadAllText(filePath, Encoding.GetEncoding(936));
                // 写入文件内容（使用UTF-8无BOM编码）
                File.WriteAllText(filePath, text, new UTF8Encoding(false));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"转换文件编码失败: {filePath}, 错误: {ex.Message}");
            return false;
        }
    }
    /// <summary>
    /// 检测文件是否需要转换为UTF-8编码
    /// </summary>
    /// <param name="file">文件路径</param>
    /// <returns>是否需要转换</returns>
    public static bool IsNeedConvertToUtf8(string file)
    {
        return !DetectFileEncoding(file, "utf-8") && DetectFileEncoding(file, "gb2312");
    }

    /// <summary>
    /// 检测文件的编码是否为指定编码
    /// </summary>
    /// <param name="file">文件路径</param>
    /// <param name="name">编码名称</param>
    /// <returns>是否为指定编码</returns>
    public static bool DetectFileEncoding(string file, string name)
    {
        var encodingVerifier = Encoding.GetEncoding(name, new EncoderExceptionFallback(), new DecoderExceptionFallback());
        using (var reader = new StreamReader(file, encodingVerifier, true, 1024))
        {
            try
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                }
                return reader.CurrentEncoding.BodyName == name;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
