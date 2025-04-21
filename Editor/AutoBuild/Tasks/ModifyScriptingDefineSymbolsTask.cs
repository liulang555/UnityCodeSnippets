using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using System.Collections.Generic;

namespace AutoBuildSystem
{
    public class ModifyScriptingDefineSymbolsTask : BaseBuildTask
    {
        public ModifyScriptingDefineSymbolsTask()
        {
            TaskId = "modify_scripting_defines";
            TaskName = "修改宏定义 Scripting Define Symbols";
            Priority = (int)BuildTaskPriority.ModifyScriptingDefineSymbolsTask;
            TaskType = BuildTaskType.PreBuild;
            Status = AutoBuildTaskStatus.Pending;
        }

        public override bool Execute(AutoBuildConfig config, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;

                // 计算最终的宏定义列表
                string newDefines = CalculateScriptingDefines(platform, channel);
                // 设置宏定义
                PlayerSettings.SetScriptingDefineSymbols(platform.GetNamedBuildTarget(), newDefines);
                LogString(config,newDefines);

                Status = AutoBuildTaskStatus.Completed;
                return true;
            }
            catch (System.Exception ex)
            {
                config.Logger.LogError($"宏定义修改任务错误: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }

        private string CalculateScriptingDefines(IAutoBuildPlatform platform, IChannel channel)
        {
            // 获取平台基础的宏定义
            var definesList = platform.GetScriptingDefines();
            var channelDefines = channel.GetScriptingDefines();

            // 移除需要排除的宏定义
            foreach (var excludeDefine in channelDefines.ExcludeDefines)
            {
                if (definesList.Contains(excludeDefine))
                    definesList.Remove(excludeDefine);
            }

            // 添加需要包含的宏定义
            foreach (var includeDefine in channelDefines.IncludeDefines)
            {
                if (!definesList.Contains(includeDefine))
                    definesList.Add(includeDefine);
            }

            string newDefines = string.Join(";", definesList);

            return newDefines;
        }
    }
}

