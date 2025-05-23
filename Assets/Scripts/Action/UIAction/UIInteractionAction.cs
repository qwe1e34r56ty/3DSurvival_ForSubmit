using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIInteractionAction : IAction
{
    private Dictionary<GameObject, TextMeshProUGUI> interactionTexts;
    private Dictionary<GameObject, GameObject> curInteractionGameObjects;
    public UIInteractionAction()
    {
        interactionTexts = new();
        curInteractionGameObjects = new();
    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {
        Transform textTransform = entity.gameObject.transform.Find("Canvas/InteractionText");
        if (textTransform == null)
        {
            Logger.LogWarning($"[UIInteractionAction] [{entity.gameObject.name}] InteractionText Object not found.");
            return;
        }
        interactionTexts.Add(entity.gameObject, textTransform.GetComponent<TextMeshProUGUI>());
        interactionTexts[entity.gameObject].gameObject.SetActive(false);
        curInteractionGameObjects.Add(entity.gameObject, null);
    }

    public void Detach(GameContext gameContext, Entity entity)
    {
        interactionTexts.Remove(entity.gameObject);
        curInteractionGameObjects.Remove(entity.gameObject);
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
            if (!curInteractionGameObjects.TryGetValue(entity.gameObject, out var curInteractionGameObject))
            {
                return;
            }
            // hit GameObject가 현재 상호작용 중인 Object와 동일한지 검사
            if (hit.collider.gameObject != curInteractionGameObject)
            {
                // 자기 자신 상호작용 금지 && hit GameObject를 포함한 Entity 존재 여부 검사 && 해당 Entity 상호작용 관련 데이터 존재 여부 검사
                if (hit.collider.gameObject != gameContext.controllableEntity.gameObject &&
                    gameContext.entities.TryGetValue(hit.collider.gameObject, out var curInteractionGameEntity) &&
                    curInteractionGameEntity.GetDescription(DescriptionID.Interaction) != null)
                {
                    interactionTexts[entity.gameObject].gameObject.SetActive(true);
                    curInteractionGameObject = hit.collider.gameObject;
                    interactionTexts[entity.gameObject].SetText(curInteractionGameEntity.GetDescription(DescriptionID.Interaction));
                }
            }
            ProcessInteraction(gameContext, curInteractionGameObject);
        }
        else
        {
            curInteractionGameObjects[entity.gameObject] = null;
            interactionTexts[entity.gameObject].gameObject.SetActive(false);
        }
    }

    private void ProcessInteraction(GameContext gameContext, GameObject target)
    {
        bool interaction = Input.GetKeyDown(KeyCode.E);
        bool changeBody = Input.GetKeyDown(KeyCode.R);
        if (interaction)
        {
            Entity controllableEntity = gameContext.controllableEntity;
            if (controllableEntity == null)
            {
                return;
            }
            if(gameContext.entities.TryGetValue(target, out var targetEntity))
            {
                string itemID = targetEntity.GetDescription(DescriptionID.ItemID);
                if (itemID == null){
                    return;
                }
                float itemCount = controllableEntity.GetStat(itemID) ?? 0;
                controllableEntity.SetStat(itemID, itemCount + 1);
                Logger.Log($"[UIInteractionAction] : [{controllableEntity.gameObject.name}] , [{itemID}] : {controllableEntity.GetStat(itemID)}");
                EntityFactory.Destroy(gameContext, targetEntity);
            }
        }
        if (changeBody)
        {
            Entity controllableEntity = gameContext.controllableEntity;
            if (controllableEntity == null)
            {
                return;
            }
            if (gameContext.entities.TryGetValue(target, out var targetEntity))
            {
                targetEntity.AttachAction(gameContext, gameContext.actions[ActionID.MoveControllableAction], 0);
            }
        }
    }
}