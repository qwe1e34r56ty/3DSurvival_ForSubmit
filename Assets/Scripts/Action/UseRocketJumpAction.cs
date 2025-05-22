using System.Collections.Generic;
using UnityEngine;

public class UseRocketJumpAction : IAction
{
    private static float defaultRocketJumpForce = 500f;
    public UseRocketJumpAction()
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
        if (!Input.GetKeyDown(KeyCode.Alpha3))
        {
            return;
        }

        float? itemCount = entity.GetStat(ItemID.RocketJump);
        if (itemCount.HasValue && itemCount.Value >= 1)
        {
            Rigidbody rigidBody = entity.gameObject.GetComponent<Rigidbody>();
            rigidBody.AddForce(Vector3.up * (entity.GetStat(StatID.RocketJumpForce) ?? defaultRocketJumpForce), ForceMode.Impulse);
            Logger.Log($"[RocketJump] used : [{entity.gameObject.name}]");
            entity.SetStat(ItemID.RocketJump, itemCount.Value - 1);
        }
    }
}
