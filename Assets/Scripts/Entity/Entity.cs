using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Entity : IUpdatable
{
    public GameObject gameObject { get; private set; }
    private SortedList<int, List<IAction>> sortedActionList;
    private Dictionary<string, float> stats;
    private Dictionary<string, string> descriptions;

    public Entity(GameContext gameContext, EntityData entityData, Vector3 offsetPosition, Vector3 offsetRotationEuler, Vector3 offsetScale)
    {
        if (entityData.prefab == null)
        {
            Logger.LogError("Prefab is null.");
            return;
        }
        gameObject = GameObject.Instantiate(entityData.prefab);
        Transform transform = gameObject.transform;
        transform.position += offsetPosition;
        transform.rotation *= Quaternion.Euler(offsetRotationEuler);
        transform.localScale = Vector3.Scale(transform.localScale, offsetScale);
        gameObject.name = entityData.prefab.name;

        sortedActionList = new SortedList<int, List<IAction>>();
        foreach (var entry in entityData.actionIDWithPriorityList)
        {
            int priority = entry.priority;
            if (gameContext.actions.TryGetValue(entry.actionID, out var action)) {
                AttachAction(gameContext, action, priority);
            }
        }
        stats = new Dictionary<string, float>();
        foreach (var entry in entityData.statKeyWithValueList)
        {
            stats.Add(entry.key, entry.value);
        }
        descriptions = new Dictionary<string, string>();
        foreach (var entry in entityData.descriptionKeyWithValueList)
        {
            descriptions.Add(entry.key, entry.value);
        }
    }


    public void Destroy(GameContext gameContext)
    {
        foreach (var pair in sortedActionList.ToList())
        {
            foreach (var action in pair.Value.ToList())
            {
                DetachAction(gameContext, action);
            }
        }

        sortedActionList.Clear();
        GameObject.Destroy(gameObject);
    }


    public void Update(GameContext gameContext, float deltaTime)
    {
        foreach (var pair in sortedActionList)
        {
            foreach (var action in pair.Value)
            {
                if (action.CanExecute(gameContext, this, deltaTime))
                {
                    action.Execute(gameContext, this, deltaTime);
                }
            }
        }
    }

    public void AttachAction(GameContext gameContext, IAction action, int priority)
    {
        if (!sortedActionList.TryGetValue(priority, out var list))
        {
            list = new List<IAction>();
            sortedActionList[priority] = list;
        }
        action.Attach(gameContext, this, priority);
        list.Add(action);
    }

    public void DetachAction(GameContext gameContext, IAction actionToRemove)
    {
        var oldActionList = new List<(int, List<IAction>)> ();
        foreach (var pair in sortedActionList)
        {
            oldActionList.Add((pair.Key, pair.Value));
        }

        foreach (var pair in oldActionList)
        {
            foreach (var action in pair.Item2)
            {
                action.Detach(gameContext, this);
            }
        }

        ClearAction();

        foreach (var pair in oldActionList)
        {
            int priority = pair.Item1;
            foreach (var action in pair.Item2)
            {
                if (action == actionToRemove)
                    continue;

                AttachAction(gameContext, action, priority);
            }
        }
    }

    public void ClearAction()
    {
        sortedActionList.Clear();
    }

    public void AddStat(string key, float value)
    {

    }
    public float? GetStat(string StatID)
    {
        if (stats.ContainsKey(StatID))
        {
            return stats[StatID];
        }
        return null;
    }

    public float? SetStat(string StatID, float value)
    {
        stats[StatID] = value;
        return stats[StatID];
    }


    public string? GetDescription(string descriptionID)
    {
        if (descriptions.ContainsKey(descriptionID))
        {
            return descriptions[descriptionID];
        }
        return null;
    }

    public string? SetDescription(string descriptionID, string value)
    {
        if (stats.ContainsKey(descriptionID))
        {
            descriptions[descriptionID] = value;
            return descriptions[descriptionID];
        }
        return null;
    }
}
