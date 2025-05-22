using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedBoostSlotAction : IAction
{
    private Dictionary<GameObject, TextMeshProUGUI> slotTexts;

    public UISpeedBoostSlotAction()
    {
        slotTexts = new();
    }

    public void Attach(GameContext context,
        Entity entity,
        int priority)
    {
        Transform slotTransform = entity.gameObject.transform.Find("Canvas/ItemUI/ItemSlots/SpeedBoost/QuantityText");
        if (slotTransform == null)
        {
            Logger.LogWarning($"[UISpeedBoostSlotAction] [{entity.gameObject.name}] SpeedBoostSlot Object not found.");
            return;
        }
        slotTexts.Add(entity.gameObject, slotTransform.GetComponent<TextMeshProUGUI>());
    }

    public void Detach(GameContext context, Entity entity)
    {
        slotTexts.Remove(entity.gameObject);
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
        if (!slotTexts.TryGetValue(entity.gameObject, out var slotText))
        {
            return;
        }
        float itemCount = context.controllableEntity.GetStat(ItemID.SpeedBoost) ?? 0;
        slotText.SetText(itemCount.ToString());
    }
}