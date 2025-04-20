using UnityEngine;
#if UseHybridCLR
using HybridCLR.Editor.Installer;
#endif

namespace AutoBuildSystem
{
    public class AutoHybridCLRTask : BaseBuildTask
    {
        public AutoHybridCLRTask()
        {
            TaskId = "hybrid_clr";
            TaskName = "生成热更新代码 HybridCLR GenerateAll";
            Priority = (int)BuildTaskPriority.AutoHybridCLRTask;
            TaskType = BuildTaskType.PreBuild;
            Status = AutoBuildTaskStatus.Pending;
        }

        public override bool Execute(AutoBuildConfig autoBuildConfig, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;
                bool needBuildBundle = autoBuildConfig.GetParameter<bool>(BuildParameterKeys.NeedBuildBundle);
                if (needBuildBundle)
                {
#if UseHybridCLR
                    autoBuildConfig.Logger.Log($"开始生成 HybridCLR 热更新代码，强制生成");
                    HybridCLR.Editor.BuildAssetsCommand.GenerateAll();
#endif
                }

                Status = AutoBuildTaskStatus.Completed;
                return true;
            }
            catch (System.Exception ex)
            {
                autoBuildConfig.Logger.LogError($"HybridCLR 任务错误: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
    }
}

