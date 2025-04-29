using ConfigTool;
using DownLoadModule;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AutoBuildSystem
{
    public class M3u8TsUrlReplacerTask : BaseBuildTask
    {
        public M3u8TsUrlReplacerTask()
        {
            TaskId = "M3u8TsUrlReplacer";
            TaskName = "修改m3u8的ts文件地址为服务器地址";
            Priority = (int)BuildTaskPriority.M3u8TsUrlReplacer;
            TaskType = BuildTaskType.PreBuild;
        }
        public override bool ExecuteInternal(Context config, IAutoBuildPlatform platform, IChannel channel)
        {
            base.LogParameter(config, BuildParameterKeys.M3u8ReplaceServerAddress);
            base.LogParameter(config, BuildParameterKeys.M3u8SourceFileFloder);
            base.LogParameter(config, BuildParameterKeys.M3u8TargetFileFloder);
            string M3u8FileFloder = config.GetParameter<string>(BuildParameterKeys.M3u8SourceFileFloder);
            string M3u8TargetFileFloder = config.GetParameter<string>(BuildParameterKeys.M3u8TargetFileFloder);
            string M3u8ReplaceServerAddress = config.GetParameter<string>(BuildParameterKeys.M3u8ReplaceServerAddress);
            ReplaceTsUrls(M3u8FileFloder,M3u8TargetFileFloder, M3u8ReplaceServerAddress);
            return true;
        }
        private void ReplaceTsUrls(string sourceFolder, string targetFolder, string serverAddress)
        {
            if (string.IsNullOrEmpty(sourceFolder) || string.IsNullOrEmpty(serverAddress))
            {
                EditorUtility.DisplayDialog("Error", "Please provide a valid folder path and server address.", "OK");
                return;
            }
            //Debug.Log($"ReplaceTsUrls , sourceFolder: {sourceFolder} ,serverAddress :{serverAddress}");
            string[] m3u8Files = Directory.GetFiles(sourceFolder, "*" + DownloadConfig.M3U8FormatName, SearchOption.AllDirectories);
            foreach (string m3u8File in m3u8Files)
            {
                //Debug.Log($"m3u8File: {m3u8File}");
                string fileName = Path.GetFileName(m3u8File);//文件名
                //获取父文件夹名字                                                  
                string parentPath = Path.GetDirectoryName(m3u8File);
                if (string.IsNullOrEmpty(parentPath))
                {
                    EditorUtility.DisplayDialog("Error", "parentPath not found.", "OK");
                    return;
                }
                string parentFolderNameFromPath = Path.GetFileName(parentPath);
                //算出保存的路径
                string targetFilePath = Path.Combine(targetFolder, parentFolderNameFromPath, fileName + ".bytes");
                YooAsset.Editor.EditorTools.CreateFileDirectory(targetFilePath); // 确保目录存在
                string url = serverAddress + parentFolderNameFromPath + "/";

                string fileContent = File.ReadAllText(m3u8File);
                fileContent = Regex.Replace(fileContent, @"^([^#].*\.ts)$", match => url + match.Groups[1].Value, RegexOptions.Multiline);//修改内容
                File.WriteAllText(targetFilePath, fileContent);
            }
            AssetDatabase.Refresh();
        }
    }
}

