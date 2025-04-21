using System.IO;
using UnityEditor;
using UnityEngine;

namespace AutoBuildSystem
{
    public class OpenBuildDirectoryTask : BaseBuildTask
    {
        public OpenBuildDirectoryTask()
        {
            TaskId = "open_build_directory";
            TaskName = "打包结束，打开输出目录";
            Priority = (int)BuildTaskPriority.OpenBuildDirectoryTask;
            TaskType = BuildTaskType.PostBuild;
            Status = AutoBuildTaskStatus.Pending;
        }

        public override bool Execute(AutoBuildConfig autoBuildConfig, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;

                base.LogParameter(autoBuildConfig, BuildParameterKeys.OpenDirectory);
                string OpenBuildDirectory = autoBuildConfig.GetParameter<string>(BuildParameterKeys.OpenDirectory);
                if (!string.IsNullOrEmpty(OpenBuildDirectory))
                {
                    EditorUtility.RevealInFinder(OpenBuildDirectory);
                    LogString(autoBuildConfig , "已打开输出目录");
                }
                else
                {
                    autoBuildConfig.Logger.LogWarning("无法打开构建目录: 未找到输出路径参数");
                }

                Status = AutoBuildTaskStatus.Completed;
                return true;
            }
            catch (System.Exception ex)
            {
                autoBuildConfig.Logger.LogError($"打开构建目录任务失败: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
    }
}