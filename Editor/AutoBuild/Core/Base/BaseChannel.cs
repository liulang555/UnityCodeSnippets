using System.Collections.Generic;

namespace AutoBuildSystem
{
    public abstract class BaseChannel : IChannel
    {
        public string ChannelName { get; protected set; }
        public string ProductName { get; protected set; }
        public ChannelCodeType ChannelId { get; protected set; }
        public string PackgeName { get; protected set; }
        public abstract List<IBuildTask> GetChannelTasks();
        public abstract ScriptingDefineSettings GetScriptingDefines();
        public abstract void InitializeChannelParameters(Context context);
    }
}