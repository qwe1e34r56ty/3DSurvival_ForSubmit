using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Entity", menuName = "New Entity")]
public class EntityData : ScriptableObject
{
    [Header("Info")]
    public GameObject prefab;

    [Header("Action")]
    public List<ActionEntry> actionIDWithPriorityList;

    [Header("Stat")]
    public List<StatEntry> statKeyWithValueList;

    [Header("Description")]
    public List<DescriptionEntry> descriptionKeyWithValueList;
}

[System.Serializable]
public class ActionEntry
{
    public string actionID;
    public int priority;
}

[System.Serializable]
public class StatEntry
{
    public string key;
    public float value;
}

[System.Serializable]
public class DescriptionEntry
{
    public string key;
    public string value;
}