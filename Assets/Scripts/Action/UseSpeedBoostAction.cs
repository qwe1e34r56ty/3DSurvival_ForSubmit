using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseSpeedBoostAction : IAction
{
    private Dictionary<Entity, Coroutine> activeEffects;
    private float defaultSpeedBoostDuration = 5f;

    public UseSpeedBoostAction()
    {
        activeEffects = new();
    }

    public bool CanExecute(GameContext gameContext, Entity entity, float deltaTime)
    {
        return true;
    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {

    }

    public void Detach(GameContext gameContext, Entity entity)
    {
        if (activeEffects.TryGetValue(entity, out Coroutine routine))
        {
            gameContext.coroutineHandler.StopRunningCoroutine(routine);
            activeEffects.Remove(entity);
        }
    }

    public void Execute(GameContext gameContext, Entity entity, float deltaTime)
    {
        if (!Input.GetKeyDown(KeyCode.Alpha2))
        {
            return;
        }

        float? speedBoostItemCount = entity.GetStat(ItemID.SpeedBoost);
        if (speedBoostItemCount.HasValue && speedBoostItemCount.Value >= 1)
        {
            entity.SetStat(ItemID.SpeedBoost, speedBoostItemCount.Value - 1);
            // 이미 사용한 SpeedBoost Item 있을 시 연관된 종료 Coroutine 중지
            if (activeEffects.TryGetValue(entity, out Coroutine existing))
            {
                gameContext.coroutineHandler.StopRunningCoroutine(existing);
                Logger.Log($"[SpeedBoost] refreshed : [{entity.gameObject.name}]");
            }
            else
            {
                float currentMoveSpeedScale = entity.GetStat(StatID.MoveSpeedScale) ?? 1f;
                entity.SetStat(StatID.MoveSpeedScale, currentMoveSpeedScale * 4);
                Logger.Log($"[SpeedBoost] used : [{entity.gameObject.name}]");
            }

                float duration = entity.GetStat(StatID.SpeedBoostDuration) ?? defaultSpeedBoostDuration;
            Coroutine newEffect = gameContext.coroutineHandler.RunCoroutine(RemoveEffectAfterDuration(gameContext, entity, duration));
            activeEffects[entity] = newEffect;
        }
    }

    private IEnumerator RemoveEffectAfterDuration(GameContext gameContext, Entity entity, float duration)
    {
        yield return new WaitForSeconds(duration);

        float currentMoveSpeedScale = entity.GetStat(StatID.MoveSpeedScale) ?? 1f;
        entity.SetStat(StatID.MoveSpeedScale, currentMoveSpeedScale / 4);
        activeEffects.Remove(entity);
        Logger.Log($"[SpeedBoost] effect off : [{entity.gameObject.name}]");
    }
}
