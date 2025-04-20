using System.IO;
using UnityEngine;

namespace AutoBuildSystem
{
    public class SteamAppIDTask : BaseBuildTask
    {
        public SteamAppIDTask()
        {
            TaskId = "steam_appid";
            TaskName = "创建 Steam AppID 文件";
            Priority = (int)BuildTaskPriority.SteamAppIDTask;
            TaskType = BuildTaskType.PostBuild;
            Status = AutoBuildTaskStatus.Pending;
        }
        public override bool Execute(AutoBuildConfig config, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;
                int steamAppID = config.GetParameter<int>(BuildParameterKeys.SteamAppID);
                string windwosFloderPath = config.GetParameter<string>(BuildParameterKeys.WindwosFloderPath);

                string strSteamAppIdPath = Path.Combine(windwosFloderPath, "steam_appid.txt");
                if (!File.Exists(strSteamAppIdPath))
                {
                    using (StreamWriter appIdFile = File.CreateText(strSteamAppIdPath))
                    {
                        appIdFile.Write(steamAppID);
                    }
                    config.Logger.Log($"AutoBuild SteamAppIDTask 创建 steam_appid.txt: {steamAppID}");
                }
                else
                {
                    config.Logger.Log("AutoBuild SteamAppIDTask steam_appid.txt 已存在");
                }

                Status = AutoBuildTaskStatus.Completed;
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"AutoBuild SteamAppIDTask 错误: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
    }
}
