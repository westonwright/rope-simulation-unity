using System;

public class RopeActionExecution
{
    private int order;
    private Action action;
    public int Order { get { return order; } }
    public Action Action { get { return action; } }

    public RopeActionExecution(int order, Action action)
    {
        this.order = order;
        this.action = action;
    }
}
