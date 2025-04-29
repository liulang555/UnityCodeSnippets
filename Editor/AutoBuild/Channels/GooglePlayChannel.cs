using System.Collections.Generic;

namespace AutoBuildSystem
{
    public class GooglePlayChannel : BaseChannel
    {
        public GooglePlayChannel()
        {
            ChannelId = ChannelCodeType.GooglePlay;
            ChannelName = "Google Play";
            PackgeName = "com.innovationdreamtech.tsr";
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