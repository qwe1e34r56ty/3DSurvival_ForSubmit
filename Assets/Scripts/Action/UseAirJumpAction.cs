using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAirJumpAction : IAction
{
    private Dictionary<Entity, Coroutine> activeEffects;
    private static float defaultAirJumpDuration = 5f;

    public UseAirJumpAction()
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
        if (!Input.GetKeyDown(KeyCode.Alpha1))
        {
            return;
        }

        float? airJumpItemCount = entity.GetStat(ItemID.AirJump);
        if (airJumpItemCount.HasValue && airJumpItemCount.Value >= 1)
        {
            entity.SetStat(ItemID.AirJump, airJumpItemCount.Value - 1);
            // 이미 사용한 AirJump Item 있을 시 연관된 종료 Coroutine 중지
            if (activeEffects.TryGetValue(entity, out Coroutine existing))
            {
                gameContext.coroutineHandler.StopRunningCoroutine(existing);
                Logger.Log($"[AirJump] refreshed : [{entity.gameObject.name}]");
            }
            else
            {
                float currentMaxAirJumpCount = entity.GetStat(StatID.MaxAirJumpCount) ?? 0;
                entity.SetStat(StatID.MaxAirJumpCount, currentMaxAirJumpCount + 1);
                Logger.Log($"[AirJump] used : [{entity.gameObject.name}]");
            }

            float duration = entity.GetStat(StatID.AirJumpDuration) ?? defaultAirJumpDuration;
            Coroutine newEffect = gameContext.coroutineHandler.RunCoroutine(RemoveEffectAfterDuration(gameContext, entity, duration));
            activeEffects[entity] = newEffect;
        }
    }

    private IEnumerator RemoveEffectAfterDuration(GameContext gameContext, Entity entity, float duration)
    {
        yield return new WaitForSeconds(duration);

        float currentMaxAirJumpCount = entity.GetStat(StatID.MaxAirJumpCount) ?? 0;
        entity.SetStat(StatID.MaxAirJumpCount, Mathf.Max(0, currentMaxAirJumpCount - 1));
        activeEffects.Remove(entity);
        Logger.Log($"[AirJump] effect off : [{entity.gameObject.name}]");
    }
}
