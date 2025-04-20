using UnityEngine;

namespace AutoBuildSystem
{
    public class VersionAddTask : BaseBuildTask
    {
        public VersionAddTask()
        {
            TaskId = "version_add";
            TaskName = "自动增加 版本号";
            Priority = (int)BuildTaskPriority.VersionAddTask;
            TaskType = BuildTaskType.PreBuild;
            Status = AutoBuildTaskStatus.Pending;
        }
        public override bool Execute(AutoBuildConfig config, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;

                // 在这里添加版本号增加的逻辑
                // TODO: 实现版本号增加的具体逻辑

                Status = AutoBuildTaskStatus.Completed;
                return true;
            }
            catch (System.Exception ex)
            {
                config.Logger.LogError($"AutoBuild VersionAddTask 错误: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
    }
}

