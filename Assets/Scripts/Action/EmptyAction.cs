public class EmptyAction : IAction
{
    public EmptyAction()
    {

    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {
        Logger.Log($"[EmptyAction] {entity.gameObject.name}");
    }

    public void Detach(GameContext gameContext, Entity entity)
    {
    }

    public bool CanExecute(GameContext gameContext, Entity entity, float deltaTime)
    {
        return true;
    }

    public void Execute(GameContext gameContext, Entity entity, float deltaTime)
    {
    }
}