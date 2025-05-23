using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIExamineAction : IAction
{
    private Dictionary<GameObject, TextMeshProUGUI> examineTexts;
    private Dictionary<GameObject, GameObject> curExamineGameObjects;
    public UIExamineAction()
    {
        examineTexts = new();
        curExamineGameObjects = new();
    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {
        Transform textTransform = entity.gameObject.transform.Find("Canvas/ExamineText");
        if (textTransform == null)
        {
            Logger.LogWarning($"[UIExamineAction] [{entity.gameObject.name}] ExamineText Object not found.");
            return;
        }
        examineTexts.Add(entity.gameObject, textTransform.GetComponent<TextMeshProUGUI>());
        examineTexts[entity.gameObject].gameObject.SetActive(false);
        curExamineGameObjects.Add(entity.gameObject, null);
    }

    public void Detach(GameContext gameContext, Entity entity)
    {
        examineTexts.Remove(entity.gameObject);
        curExamineGameObjects.Remove(entity.gameObject);
    }

    public bool CanExecute(GameContext gameContext, Entity entity, float deltaTIme)
    {
        return true;
    }

    public void Execute(GameContext gameContext, Entity entity, float deltaTIme)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out var hit, 17.0f))
        {
            if (!curExamineGameObjects.TryGetValue(entity.gameObject, out var curExamineGameObject))
            {
                return;
            }
            if (hit.collider.gameObject == curExamineGameObject ||
                hit.collider.gameObject == gameContext.controllableEntity.gameObject)
            {
                return;
            }
            if (!gameContext.entities.TryGetValue(hit.collider.gameObject, out var curExamineGameEntity))
            {
                return;
            }
            if (curExamineGameEntity.GetDescription(DescriptionID.Examine) == null)
            {
                return;
            }
            examineTexts[entity.gameObject].gameObject.SetActive(true);
            curExamineGameObject = hit.collider.gameObject;
            examineTexts[entity.gameObject].SetText(curExamineGameEntity.GetDescription(DescriptionID.Examine));
        }
        else
        {
            curExamineGameObjects[entity.gameObject] = null;
            examineTexts[entity.gameObject].gameObject.SetActive(false);
        }
    }
}