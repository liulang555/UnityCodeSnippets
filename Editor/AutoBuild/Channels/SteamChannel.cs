using System.Collections.Generic;

namespace AutoBuildSystem
{
    public class SteamChannel : BaseChannel
    {
        public SteamChannel()
        {
            ChannelId = ChannelCodeType.Steam;
            ChannelName = "Steam";
            PackgeName = "com.InnovationDream.TimeRebuild";
            ProductName = "時空重構";
        }
        
        public override List<IBuildTask> GetChannelTasks()
        {
            return new List<IBuildTask>
            {
                new SteamAppIDTask(),
            };
        }
        public override ScriptingDefineSettings GetScriptingDefines()
        {
            return new ScriptingDefineSettings
            {
                IncludeDefines = new HashSet<string> { "STEAMWORKS_NET" },
                ExcludeDefines = new HashSet<string> { }
            };
        }

        public override void InitializeChannelParameters(Context context)
        {
            context.SetParameter(BuildParameterKeys.SteamAppID, 3292340);
        }
    }
} 