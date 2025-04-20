using System.Collections.Generic;

namespace AutoBuildSystem
{
    public class ScriptingDefineSettings
    {
        public HashSet<string> IncludeDefines { get; set; } = new HashSet<string>();
        public HashSet<string> ExcludeDefines { get; set; } = new HashSet<string>();
    }

    public interface IChannel
    {
        ChannelCodeType ChannelId { get; }
        string ChannelName { get; }
        ScriptingDefineSettings GetScriptingDefines();
        /// <summary>
        /// 包名
        /// </summary>
        string PackgeName { get; }
        /// <summary>
        /// 应用的名字 显示给玩家的
        /// </summary>
        string ProductName { get; }
        List<IBuildTask> GetChannelTasks();
        
        /// <summary>
        /// 设置渠道特定的参数到共享参数字典
        /// </summary>
        /// <param name="autoBuildConfig">构建配置</param>
        void InitializeChannelParameters(AutoBuildConfig autoBuildConfig);
    }
}