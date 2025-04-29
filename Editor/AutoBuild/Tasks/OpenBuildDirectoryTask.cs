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
        }

        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                base.LogParameter(context, BuildParameterKeys.OpenDirectory);
                string OpenBuildDirectory = context.GetParameter<string>(BuildParameterKeys.OpenDirectory);
                if (!string.IsNullOrEmpty(OpenBuildDirectory))
                {
                    EditorUtility.RevealInFinder(OpenBuildDirectory);
                    LogString(context , "已打开输出目录");
                }
                else
                {
                    context.Logger.LogWarning("无法打开构建目录: 未找到输出路径参数");
                }
                return true;
            }
            catch (System.Exception ex)
            {
                context.Logger.LogError($"打开构建目录任务失败: {ex.Message}");
                return false;
            }
        }
    }
}