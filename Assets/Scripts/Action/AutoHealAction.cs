using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoHealAction : IAction
{
    public AutoHealAction()
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
        float? hp = entity.GetStat(StatID.Hp);
        float? maxHp = entity.GetStat(StatID.MaxHp);
        float? autoHeal = entity.GetStat(StatID.AutoHeal);
        if (!hp.HasValue)
        {
            return;
        }
        if (!maxHp.HasValue)
        {
            return;
        }
        if (!autoHeal.HasValue)
        {
            return;
        }
        if (hp > maxHp)
        {
            return;
        }
        entity.SetStat(StatID.Hp, 
            Mathf.Min(
                hp.Value + autoHeal.Value * deltaTime, 
                maxHp.Value
                )
            );
    }
}