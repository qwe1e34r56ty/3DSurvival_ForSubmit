using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "New Scene")]
public class SceneData : ScriptableObject
{
    [Header("Info")]
    public List<EntityEntry> entityDataList;

}

[System.Serializable]
public class EntityEntry
{
    public EntityData entityData;
    public Vector3 offsetPosition;
    public Vector3 offsetRotationEuler;
    public Vector3 offsetScale;
}