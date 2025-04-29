using System.Collections.Generic;

namespace AutoBuildSystem
{
    public class LocalTestChannel : BaseChannel
    {
        public LocalTestChannel()
        {
            ChannelId = ChannelCodeType.LocalTest;
            ChannelName = "Local Test";
            PackgeName = "com.InnovationDream.TimeRebuild";
            ProductName = "時空重構";
        }

        public override List<IBuildTask> GetChannelTasks()
        {
            return new List<IBuildTask>
            {

            };
        }

        public override ScriptingDefineSettings GetScriptingDefines()
        {
            return new ScriptingDefineSettings
            {
                IncludeDefines = new HashSet<string> { "STEAMWORKS_NET" },//防止切换steam平台报错   本地测试直接用一套宏定义
                ExcludeDefines = new HashSet<string> { }
            };
        }
        public override void InitializeChannelParameters(Context context)
        {

        }
    }
}