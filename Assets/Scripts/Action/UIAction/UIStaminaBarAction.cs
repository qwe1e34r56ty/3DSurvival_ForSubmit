using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBarAction : IAction
{
    private Dictionary<GameObject, Image> barImages;

    public UIStaminaBarAction()
    {
        barImages = new();
    }

    public void Attach(GameContext context, 
        Entity entity, 
        int priority)
    {
        Transform barTransform = entity.gameObject.transform.Find("Canvas/Conditions/Stamina/StaminaBar");
        if (barTransform == null)
        {
            Logger.LogWarning($"[UIStaminaBarAction] [{entity.gameObject.name}] StaminaBar Object not found.");
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
        float stamina = context.controllableEntity.GetStat(StatID.Stamina) ?? 0;
        float maxStamina = context.controllableEntity.GetStat(StatID.MaxStamina) ?? 0;
        if(maxStamina == 0)
        {
            return;
        }
        image.fillAmount = Mathf.Clamp01(stamina / maxStamina);
    }
}