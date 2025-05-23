using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAirJumpSlotAction : IAction
{
    private Dictionary<GameObject, TextMeshProUGUI> slotTexts;

    public UIAirJumpSlotAction()
    {
        slotTexts = new();
    }

    public void Attach(GameContext context,
        Entity entity,
        int priority)
    {
        Transform slotTransform = entity.gameObject.transform.Find("Canvas/ItemUI/ItemSlots/AirJump/QuantityText");
        if (slotTransform == null)
        {
            Logger.LogWarning($"[UIAirJumpSlotAction] [{entity.gameObject.name}] AirJumpSlot Object not found.");
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
        float itemCount = context.controllableEntity.GetStat(ItemID.AirJump) ?? 0;
        slotText.SetText(itemCount.ToString());
    }
}