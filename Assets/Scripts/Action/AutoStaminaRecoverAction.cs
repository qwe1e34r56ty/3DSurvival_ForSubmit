using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoStaminaRecoverAction : IAction
{
    public AutoStaminaRecoverAction()
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
        float? stamina = entity.GetStat(StatID.Stamina);
        float? maxStamina = entity.GetStat(StatID.MaxStamina);
        float? autoStaminaRecover = entity.GetStat(StatID.AutoStaminaRecover);
        if (!stamina.HasValue ||
            !maxStamina.HasValue ||
            !autoStaminaRecover.HasValue ||
            stamina > maxStamina)
        {
            return; 
        }
        entity.SetStat(StatID.Stamina, 
            Mathf.Min(
                stamina.Value + autoStaminaRecover.Value * deltaTime, 
                maxStamina.Value
                )
            );
    }
}