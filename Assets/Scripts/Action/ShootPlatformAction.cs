using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ShootPlatformAction : IAction
{
    private Dictionary<GameObject, CollisionListener> collisionListeners;
    private Dictionary<GameObject, HashSet<GameObject>> aboveGameObjects;

    public ShootPlatformAction()
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

    public void Execute(GameContext gameContext, Entity entity, float deltaTIme)
    {
        bool shoot = Input.GetKeyDown(KeyCode.K);
        if (!shoot)
        {
            return;
        }
        foreach (var pair in aboveGameObjects)
        {
            foreach(var gameObject in pair.Value)
            {
                if(gameObject == null)
                {
                    continue;
                }
                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    Vector3 launchDirection = Vector3.up + pair.Key.transform.forward * 0.2f;
                    float launchPower = entity.GetStat(StatID.ShootPlatformForce) ?? 0;
                    rigidbody.AddForce(launchDirection.normalized * launchPower, ForceMode.Impulse);
                }
            }
        }
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
