namespace EvaluateBT
{
    public enum EParallelFinishCondition
    {
        Or,
        And,
    }

    public enum EStatusBTRunning
    {
        Executing,
        Finish,
        ErrorTransition
    }

    public enum EStatusNodeTerminal
    {
        Ready,
        Running,
        Finish,
    }
}