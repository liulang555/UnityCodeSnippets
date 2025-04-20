using System.Threading.Tasks;

namespace AutoBuildSystem
{
    public interface IBuildTask
    {
        string TaskId { get; }
        string TaskName { get; }
        int Priority { get; }
        BuildTaskType TaskType { get; }
        AutoBuildTaskStatus Status { get; }
        
        /// <summary>
        /// 执行构建任务
        /// </summary>
        /// <param name="autoBuildConfig">构建配置，包含共享参数字典</param>
        /// <param name="platform">构建平台</param>
        /// <param name="channel">构建渠道</param>
        /// <returns>任务是否成功执行</returns>
        bool Execute(AutoBuildConfig autoBuildConfig, IAutoBuildPlatform platform, IChannel channel);
        
        void Cancel();
    }
}