using System.Threading.Tasks;

namespace AutoBuildSystem
{
    public interface IBuildTask
    {
        string TaskId { get; }
        string TaskName { get; }
        int Priority { get; }
        BuildTaskType TaskType { get; }
        int BuildTimes { get; }
        /// <summary>
        /// 执行构建任务  父类添加通用逻辑
        /// </summary>
        bool Execute(Context context, IAutoBuildPlatform platform, IChannel channel);
        /// <summary>
        /// 执行构建任务  子类实现
        /// </summary>
        bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel);
    }
}