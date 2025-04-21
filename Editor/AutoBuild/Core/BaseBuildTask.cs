using System.Threading.Tasks;
using UnityEngine;

namespace AutoBuildSystem
{
    public abstract class BaseBuildTask : IBuildTask
    {
        public string TaskId { get; protected set; }
        public string TaskName { get; protected set; }
        public int Priority { get; protected set; }
        public BuildTaskType TaskType { get; protected set; }
        public AutoBuildTaskStatus Status { get; protected set; }
        public abstract bool Execute(AutoBuildConfig autoBuildConfig, IAutoBuildPlatform platform, IChannel channel);
        public virtual void Cancel()
        {
            Status = AutoBuildTaskStatus.Cancelled;
        }
        // 打印任务参数
        protected void LogParameter(AutoBuildConfig autoBuildConfig, BuildParameterKeys paramkey)
        {
            autoBuildConfig.Logger.Log($"----- [{TaskId}] 参数: {paramkey} , 值：{autoBuildConfig.GetParameter<object>(paramkey)}");
        }
        protected void LogString(AutoBuildConfig autoBuildConfig, string str)
        {
            autoBuildConfig.Logger.Log($"----- [{TaskId}] , {str}");
        }
    }
}