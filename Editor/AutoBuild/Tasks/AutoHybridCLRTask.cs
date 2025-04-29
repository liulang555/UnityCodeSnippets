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
        }

        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                base.LogParameter(context, BuildParameterKeys.NeedBuildBundle);
                bool needBuildBundle = context.GetParameter<bool>(BuildParameterKeys.NeedBuildBundle);
                if (needBuildBundle)
                {
#if UseHybridCLR
                    Debug.Log($"开始生成 HybridCLR 热更新代码，强制生成");
                    HybridCLR.Editor.BuildAssetsCommand.GenerateAll();
#endif
                }
                return true;
            }
            catch (System.Exception ex)
            {
                context.Logger.LogError($"HybridCLR 任务错误: {ex.Message}");
                return false;
            }
        }
    }
}

