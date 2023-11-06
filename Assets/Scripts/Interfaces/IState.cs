public interface IState
{
    public FiniteStateMachine Owner { get; }
    void InitializeState(FiniteStateMachine owner);
    void OnStateEnter();
    void OnStateUpdate();
    void OnStateFixedUpdate();
    void OnStateExit();

}
