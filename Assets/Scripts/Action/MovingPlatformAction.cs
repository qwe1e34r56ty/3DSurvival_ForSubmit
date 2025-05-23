using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MovingPlatformAction : IAction
{
    private Dictionary<GameObject, CollisionListener> collisionListeners;
    private Dictionary<GameObject, HashSet<GameObject>> aboveGameObjects;

    public MovingPlatformAction()
    {
        collisionListeners = new();
        aboveGameObjects = new();
    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {
        GameObject gameObject = entity.gameObject;

        if (!collisionListeners.ContainsKey(gameObject))
        {
            CollisionListener collisionListener = gameObject.GetComponent<CollisionListener>() ?? 
                gameObject.AddComponent<CollisionListener>();

            collisionListener.OnCollisionEnterCallBack = (collision) =>
            {
                OnCollisionEnter(gameObject, collision, entity);
            };
            collisionListener.OnCollisionExitCallBack = (collision) =>
            {
                OnCollisionExit(gameObject, collision, entity);
            };
            collisionListeners.Add(gameObject, collisionListener);
        }
        aboveGameObjects.Add(entity.gameObject, new());
    }

    public void Detach(GameContext gameContext, Entity entity)
    {
        GameObject gameObject = entity.gameObject;
        if (collisionListeners.TryGetValue(gameObject, out var listener))
        {
            listener.OnCollisionEnterCallBack = null;
            listener.OnCollisionExitCallBack = null;
            Object.Destroy(listener);

            collisionListeners.Remove(gameObject);
        }
        aboveGameObjects.Remove(entity.gameObject);
    }

    public bool CanExecute(GameContext gameContext, Entity entity, float deltaTIme)
    {
        return true;
    }

    public void Execute(GameContext gameContext, Entity entity, float deltaTime)
    {
        float speed = entity.GetStat(StatID.MovingPlatformSpeed) ?? 0;
        float edge = entity.GetStat(StatID.MovingEdge) ?? 0;
        float movedTime = entity.GetStat(StatID.MovedTime) ?? 0;
        movedTime += deltaTime;

        float x = Mathf.PingPong(movedTime * speed, 2f * edge) - edge;

        float previousX = entity.gameObject.transform.position.x;
        float deltaMove = x - previousX;

        Transform platformTransform = entity.gameObject.transform;
        platformTransform.position += new Vector3(deltaMove, 0f, 0f);

        foreach (var pair in aboveGameObjects)
        {
            foreach (var gameObject in pair.Value)
            {
                if (gameObject == null)
                {
                    continue;
                }
                Rigidbody aboveRigidbody = gameObject.GetComponent<Rigidbody>();
                if (aboveRigidbody != null)
                {
                    aboveRigidbody.position += new Vector3(deltaMove, 0f, 0f);
                }
            }
        }

        entity.SetStat(StatID.MovedTime, movedTime);
    }


    private void OnCollisionEnter(GameObject owner, Collision collision, Entity entity)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            aboveGameObjects[owner].Add(collision.gameObject);
        }
    }

    private void OnCollisionExit(GameObject owner, Collision collision, Entity entity)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            aboveGameObjects[owner].Remove(collision.gameObject);
        }
    }
}
