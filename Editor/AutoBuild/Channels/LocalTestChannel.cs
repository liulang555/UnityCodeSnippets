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
                IncludeDefines = new HashSet<string> { "DISABLESTEAMWORKS" },
                ExcludeDefines = new HashSet<string> { }
            };
        }
        public override void InitializeChannelParameters(AutoBuildConfig autoBuildConfig)
        {

        }
    }
}