using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpZoneAction : IAction
{
    private Dictionary<GameObject, CollisionListener> collisionListeners;

    public PlayerJumpZoneAction()
    {
        collisionListeners = new();
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

    }

    public bool CanExecute(GameContext gameContext, Entity entity, float deltaTIme)
    {
        return true;
    }

    public void Execute(GameContext gameContext, Entity entity, float deltaTIme)
    {

    }

    private void OnCollisionEnter(GameObject owner, Collision collision, Entity entity)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Vector3 launchDirection = Vector3.up + owner.transform.forward * 0.2f;
                float launchPower = entity.GetStat(StatID.JumpLaunchForce) ?? 0;

                rigidbody.AddForce(launchDirection.normalized * launchPower, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionExit(GameObject owner, Collision collision, Entity entity)
    {
    }
}
