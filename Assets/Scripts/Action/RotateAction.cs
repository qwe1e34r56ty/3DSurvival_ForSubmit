using UnityEngine;

public class RotateAction : IAction
{
    public RotateAction()
    {

    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {

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
        float? rotateSpeed = entity.GetStat(StatID.RotateSpeed);

        if (!rotateSpeed.HasValue)
        {
            return;
        }

        entity.gameObject.transform.Rotate(new Vector3(0, 0, 1), -rotateSpeed.Value * deltaTime);
    }
}