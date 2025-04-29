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
        }
        
        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            base.LogParameter(context, BuildParameterKeys.SteamAppID);
            int steamAppID = context.GetParameter<int>(BuildParameterKeys.SteamAppID);

            base.LogParameter(context, BuildParameterKeys.WindwosFloderPath);
            string windwosFloderPath = context.GetParameter<string>(BuildParameterKeys.WindwosFloderPath);

            string strSteamAppIdPath = Path.Combine(windwosFloderPath, "steam_appid.txt");
            if (!File.Exists(strSteamAppIdPath))
            {
                using (StreamWriter appIdFile = File.CreateText(strSteamAppIdPath))
                {
                    appIdFile.Write(steamAppID);
                }
                context.Logger.Log($"AutoBuild SteamAppIDTask 创建 steam_appid.txt: {steamAppID}");
            }
            else
            {
                context.Logger.Log("AutoBuild SteamAppIDTask steam_appid.txt 已存在");
            }
            return true;
        }
    }
}
