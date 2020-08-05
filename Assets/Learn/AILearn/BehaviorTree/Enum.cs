namespace AILearn
{
    namespace BehaviorTree
    {
        // 全局控制变量
        static class Global
        {
            public const int MaxChildNum = 16;
            public const int InvalidNodeIndex = MaxChildNum;
            public static bool IsRecursionOk = true;
        }

        // 并行判断类型
        public enum ParallelConditionEnum
        {
            Or = 1,
            And =2,
        };

        // 节点运行状态
        public enum TickStatus
        {
            Executing = 1,  // 执行中
            Finish = 2,  // 执行结束
            ErrorTransition = -1,  // 执行异常
        };

        enum ActionStatus
        {
            Ready = 1,  // 准备
            Running = 2,  // 执行中
            Finish = 3,  // 已完成
        };
    }
}