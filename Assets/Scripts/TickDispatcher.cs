using System.Linq;
using UnityEngine;

public class TickDispatcher
{
    public void Dispatch(GameContext gameContext)
    {
        foreach (IUpdatable updatable in gameContext.updateHandlers.ToList())
        {
            updatable.Update(gameContext, Time.deltaTime);
        }
    }
}