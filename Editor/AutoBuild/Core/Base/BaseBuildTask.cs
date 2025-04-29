using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoBuildSystem
{
    public abstract class BaseBuildTask : IBuildTask
    {
        public string TaskId { get; protected set; }
        public string TaskName { get; protected set; }
        public int Priority { get; protected set; }
        public int BuildTimes { get; protected set; }
        public BuildTaskType TaskType { get; protected set; }
        /// <summary>
        /// 执行构建任务  父类添加通用逻辑
        /// </summary>
        public bool Execute(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                // 日志
                context.Logger.Log($"----- {TaskName} -----");
                // 使用 Stopwatch 进行精确计时
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                bool success = ExecuteInternal(context, platform, channel);
                stopwatch.Stop();
                this.BuildTimes = (int)(stopwatch.ElapsedMilliseconds / 1000f);

                if (success)
                {
                    context.Logger.Log($"成功 {TaskName}  (耗时: {this.BuildTimes} 秒)");
                }
                else
                {
                    context.Logger.LogError($"失败 {TaskName}  (耗时: {this.BuildTimes} 秒)");
                }
                return success;
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"任务 {TaskName} 执行出错: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        /// <summary>
        ///  执行构建任务  子类实现(抽象方法，用于实际的任务执行逻辑)
        /// </summary>
        public abstract bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel);

        #region 打印日志
        // 打印参数日志
        protected void LogParameter(Context context, BuildParameterKeys paramkey)
        {
            if (context == null || context.Logger == null)
                return;
            context.Logger.Log($"----- [{TaskId}] 参数: {paramkey} , 值：{context.GetParameter<object>(paramkey)}");
        }

        protected void LogString(Context context, string str)
        {
            if (context == null || context.Logger == null)
                return;
            context.Logger.Log($"----- [{TaskId}] , {str}");
        }
        #endregion
    }
}