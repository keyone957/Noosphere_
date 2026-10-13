using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseRaycastDebugger : MonoBehaviour
{
    public static MouseRaycastDebugger Instance { get; private set; }
    public bool logOnClickOnly = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (logOnClickOnly && !Input.GetMouseButtonDown(0)) return;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Debug.Log($"[UI RAYCAST] hits={results.Count}");
        for (int i = 0; i < results.Count; i++)
        {
            var r = results[i];
            Debug.Log($"{i}. name={r.gameObject.name}, module={r.module?.GetType().Name}, " +
                      $"sortingLayer={r.sortingLayer}, sortingOrder={r.sortingOrder}, " +
                      $"distance={r.distance}, depth={r.depth}");
        }
    }
}
