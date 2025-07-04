﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext
{
    public Dictionary<GameObject, Entity> entities; // Entity <-> Entity 내 GameObject 매핑
    public Dictionary<string, IAction> actions; // ActionID <-> IAction 매핑
    public HashSet<IUpdatable> updateHandlers;
    public Entity controllableEntity; // 메인카메라 attach 되어 있는 Entity
    public CoroutineHandler coroutineHandler; // Corountine 호출 담당 Monobehaviour


	public GameContext()
    {
        entities = new();
        actions = new();
        initActions(actions);
        updateHandlers = new();
        controllableEntity = null;
        coroutineHandler = null;
	}
    public void initActions(Dictionary<string, IAction> actions)
    {
        actions.Add(ActionID.UpdateableAction, new UpdateableAction());
        actions.Add(ActionID.EmptyAction, new EmptyAction());
        actions.Add(ActionID.MoveControllableAction, new MoveControllableAction());
        actions.Add(ActionID.PlayerJumpZoneAction, new PlayerJumpZoneAction());
        actions.Add(ActionID.MoveableAction, new MoveableAction());
        actions.Add(ActionID.AutoHealAction, new AutoHealAction());
        actions.Add(ActionID.RotateAction, new RotateAction());
        actions.Add(ActionID.UseRocketJumpAction, new UseRocketJumpAction());
        actions.Add(ActionID.UseAirJumpAction, new UseAirJumpAction());
        actions.Add(ActionID.UseSpeedBoostAction, new UseSpeedBoostAction());
        actions.Add(ActionID.AutoStaminaRecoverAction, new AutoStaminaRecoverAction());
        actions.Add(ActionID.ShootPlatformAction, new ShootPlatformAction());
        actions.Add(ActionID.MovingPlatformAction, new MovingPlatformAction());

        actions.Add(ActionID.UIHpBarAction, new UIHpBarAction());
        actions.Add(ActionID.UIExamineAction, new UIExamineAction());
        actions.Add(ActionID.UIInteractionAction, new UIInteractionAction());
        actions.Add(ActionID.UIRocketJumpSlotAction, new UIRocketJumpSlotAction());
        actions.Add(ActionID.UISpeedBoostSlotAction, new UISpeedBoostSlotAction());
        actions.Add(ActionID.UIAirJumpSlotAction, new UIAirJumpSlotAction());
        actions.Add(ActionID.UIStaminaBarAction, new UIStaminaBarAction());
    }
    public void AddEntity(Entity entity)
    {
        entities.Add(entity.gameObject, entity);
    }
    public void RemoveEntity(Entity entity)
    {
        entities.Remove(entity.gameObject);
    }
}
