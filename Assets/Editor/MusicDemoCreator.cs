using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// Editor helper to create a demo MusicManager, a demo player and a demo enemy
// Use: Tools > Create Music Manager Demo
public static class MusicDemoCreator
{
    [MenuItem("Tools/Create Music Manager Demo")]
    public static void CreateDemo()
    {
        // Create MusicManager
        GameObject mmObj = new GameObject("MusicManager");
        Undo.RegisterCreatedObjectUndo(mmObj, "Create MusicManager");
        var mm = mmObj.AddComponent<MusicManager>();

        // Default stem names
        mm.stemParameterNames = new string[] {
            "Music_Ambient_Vol",
            "Music_Percussion_Vol",
            "Music_Bass_Vol",
            "Music_Lead_Vol"
        };

        // Create sensible default curves
        mm.stemCurves = new AnimationCurve[mm.stemParameterNames.Length];
        // Ambient: present mostly until chase (starts high, dips slightly)
        mm.stemCurves[0] = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.7f, 0.9f),
            new Keyframe(1f, 0.7f)
        );
        // Percussion: quiet at 0, comes in mid-intensity
        mm.stemCurves[1] = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.3f, 0.2f),
            new Keyframe(0.6f, 0.9f),
            new Keyframe(1f, 1f)
        );
        // Bass: subtle then ramps
        mm.stemCurves[2] = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.4f, 0.1f),
            new Keyframe(0.8f, 1f)
        );
        // Lead: appears near full intensity
        mm.stemCurves[3] = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.7f, 0.05f),
            new Keyframe(0.9f, 0.8f),
            new Keyframe(1f, 1f)
        );

        mm.rampSpeed = 1f;
        mm.maxReactionDistance = 30f;
        mm.reportTimeout = 2f;

        // Create a DemoPlayer (simple transform with Player tag)
        GameObject player = new GameObject("DemoPlayer");
        Undo.RegisterCreatedObjectUndo(player, "Create DemoPlayer");
        player.transform.position = Vector3.zero;
        // try to assign Player tag if it exists
        try { player.tag = "Player"; } catch { }

        // Create a DemoEnemy
        GameObject enemy = new GameObject("DemoEnemy");
        Undo.RegisterCreatedObjectUndo(enemy, "Create DemoEnemy");
        enemy.transform.position = new Vector3(0f, 0f, 8f);
        // Add NavMeshAgent for EnemyMove
        var agent = enemy.AddComponent<NavMeshAgent>();
        agent.speed = 3.5f;
        agent.stoppingDistance = 1.2f;

        // Add existing EnemyMove and EnemyAudioReporter
        enemy.AddComponent<EnemyMove>();
        enemy.AddComponent<EnemyAudioReporter>();

        // Select created objects in the editor
        Selection.objects = new Object[] { mmObj, player, enemy };

        EditorUtility.DisplayDialog("Music Demo Created",
            "Created MusicManager, DemoPlayer (tagged 'Player' if available), and DemoEnemy.\n\nAssign your AudioMixer to the MusicManager and expose the stem parameter names in the Mixer.",
            "OK");
    }
}
