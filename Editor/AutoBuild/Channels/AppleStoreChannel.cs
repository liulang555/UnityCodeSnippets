using System.Collections.Generic;

namespace AutoBuildSystem
{
    public class AppleStoreChannel : BaseChannel
    {
        public AppleStoreChannel()
        {
            ChannelId = ChannelCodeType.AppleStore;
            ChannelName = "Apple Store";
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
                IncludeDefines = new HashSet<string> { },
                ExcludeDefines = new HashSet<string> { }
            };
        }

        public override void InitializeChannelParameters(Context context)
        {

        }
    }
}
