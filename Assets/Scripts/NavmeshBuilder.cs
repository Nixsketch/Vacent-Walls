using UnityEngine;
using Unity.AI.Navigation;


#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Simple helper to build a NavMeshSurface at runtime or from the editor.
/// Requires the NavMeshComponents (NavMeshSurface) package from Unity Technologies.
/// Place this on a GameObject (for example a Level root) and assign a NavMeshSurface
/// or add a NavMeshSurface component to the same GameObject. Call BuildNavMesh()
/// after your procedural maze/level geometry has been created.
/// </summary>
public class NavmeshBuilder : MonoBehaviour
{
    [Tooltip("NavMeshSurface to build. If empty, this GameObject's NavMeshSurface will be used.")]
    public NavMeshSurface surface;

    [Tooltip("Automatically build the NavMesh when the scene starts (Play mode).")]
    public bool buildOnStart = true;

    void Reset()
    {
        if (surface == null)
            surface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        if (buildOnStart)
            BuildNavMesh();
    }

    /// <summary>
    /// Build the assigned NavMeshSurface. If no surface is assigned, tries to use a NavMeshSurface
    /// component attached to this GameObject.
    /// </summary>
    public void BuildNavMesh()
    {
        if (surface == null)
            surface = GetComponent<NavMeshSurface>();

        if (surface == null)
        {
            Debug.LogError("NavmeshBuilder: No NavMeshSurface found. Add a NavMeshSurface component or assign one in the inspector.");
            return;
        }

        surface.BuildNavMesh();
        Debug.Log($"NavmeshBuilder: Built NavMesh for surface '{surface.name}'.");
    }

#if UNITY_EDITOR
    // Convenient context menu so you can right-click the component in the inspector and build.
    [ContextMenu("Build NavMesh (Editor)")]
    private void BuildNavMeshContext()
    {
        BuildNavMesh();
        // If running in edit mode, mark the scene as dirty so the user can save the bake if desired.
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
#endif
}
