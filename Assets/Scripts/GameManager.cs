using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GameContext gameContext;
    private TickDispatcher tickDispatcher;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameContext = new();
        tickDispatcher = new();
        CoroutineHandler handler = this.gameObject.AddComponent<CoroutineHandler>();
        gameContext.coroutineHandler = handler;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        tickDispatcher.Dispatch(gameContext);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var copy = new List<Entity>();
        foreach(var pair in gameContext.entities)
        {
            copy.Add(pair.Value);
        }
        foreach (Entity entity in copy)
        {
            EntityFactory.Destroy(gameContext, entity);
        }
        string sceneName = scene.name;
        Debug.Log($"[GameManager] Loaded Scene: {sceneName}");
        LoadSceneObject(gameContext, sceneName);
    }

    private SceneData LoadSceneData(string sceneID)
    {
        string path = $"ScriptableObject/SceneData/{sceneID}";
        SceneData data = Resources.Load<SceneData>(path);
        return data;
    }

    private void LoadSceneObject(GameContext gameContext, string sceneID)
    {
        SceneData sceneData = LoadSceneData(sceneID);
        if (sceneData == null)
        {
            Logger.LogWarning($"[GameManager] SceneData not found for scene: {sceneID}");
            return;
        }

        foreach (EntityEntry entry in sceneData.entityDataList)
        {
            EntityFactory.Create(gameContext, entry.entityData, entry.offsetPosition, entry.offsetRotationEuler, entry.offsetScale);
        }
    }
}