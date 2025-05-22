using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityFactory
{

    public static Entity Create(GameContext gameContext, EntityData entityData, Vector3 offsetPosition, Vector3 offsetRotationEuler, Vector3 offsetScale)
    {
        Entity entity = new(gameContext, entityData, offsetPosition, offsetRotationEuler, offsetScale);
        gameContext.AddEntity(entity);
        return entity;
    }

    public static void Destroy(GameContext gameContext, Entity entity)
    {
        gameContext.RemoveEntity(entity);
        entity.Destroy(gameContext);
    }
}
