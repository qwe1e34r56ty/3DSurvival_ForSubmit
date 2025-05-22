using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHpBarAction : IAction
{
    private Dictionary<GameObject, Image> barImages;

    public UIHpBarAction()
    {
        barImages = new();
    }

    public void Attach(GameContext context, 
        Entity entity, 
        int priority)
    {
        Transform barTransform = entity.gameObject.transform.Find("Canvas/Conditions/Hp/HpBar");
        if (barTransform == null)
        {
            Logger.LogWarning($"[UIHpBarAction] [{entity.gameObject.name}] HpBar Object not found.");
            return;
        }
        barImages.Add(entity.gameObject, barTransform.GetComponent<Image>());
    }

    public void Detach(GameContext context, Entity entity)
    {
        barImages.Remove(entity.gameObject);
    }

    public bool CanExecute(GameContext context, Entity entity, float deltaTIme)
    {
        if (context.controllableEntity == null)
        {
            return false;
        }
        return true;
    }

    public void Execute(GameContext context, Entity entity, float deltaTIme)
    {
        if (!barImages.TryGetValue(entity.gameObject, out var image))
        {
            return;
        }
        float hp = context.controllableEntity.GetStat(StatID.Hp) ?? 0;
        float maxHp = context.controllableEntity.GetStat(StatID.MaxHp) ?? 0;
        if(maxHp == 0)
        {
            return;
        }
        image.fillAmount = Mathf.Clamp01(hp / maxHp);
    }
}