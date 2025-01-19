using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TreePlacerEditor : EditorWindow
{
    // Reference to the Tree-Collider prefab
    public GameObject treePrefab;

    // LayerMask for placement
    public LayerMask placementLayerMask;

    // Tool activation
    private bool isPlacing = false;

    [MenuItem("Tools/Tree Placer")]
    public static void ShowWindow()
    {
        GetWindow<TreePlacerEditor>("Tree Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tree Placer Tool", EditorStyles.boldLabel);

        // Tree prefab field
        treePrefab = (GameObject)EditorGUILayout.ObjectField("Tree Prefab", treePrefab, typeof(GameObject), false);

        // Placement LayerMask
        placementLayerMask = EditorGUILayout.LayerField("Placement Layer", placementLayerMask);

        // Button to toggle placement mode
        if (GUILayout.Button(isPlacing ? "Stop Placing Trees" : "Start Placing Trees"))
        {
            isPlacing = !isPlacing;
            SceneView.duringSceneGui -= OnSceneGUI;
            if (isPlacing)
                SceneView.duringSceneGui += OnSceneGUI;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (treePrefab == null)
        {
            Debug.LogWarning("Tree Prefab is not assigned!");
            return;
        }

        Event e = Event.current;

        // Handle mouse click in the Scene View
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << placementLayerMask))
            {
                // Place the tree at the hit point
                Undo.RegisterCreatedObjectUndo(Instantiate(treePrefab, hit.point, Quaternion.identity), "Place Tree");
            }

            e.Use();
        }
    }
}
