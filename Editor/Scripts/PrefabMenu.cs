using UnityEditor;
using UnityEngine;

namespace Aarware.Core.Editor{


public static class PrefabMenu{
    
     // Path to the prefabs in your Unity package
    static string PackagePrefabPath => ScriptCreator.packageRoot;
    const string menuRoot           = "Assets/Prefabs/";
    const string toolRoot           = "Aarware/Prefabs/";

    [MenuItem(toolRoot + "Panel", false, 1)]
    [MenuItem(menuRoot + "Panel", false, 1)]
    private static void CreatePanel(){
        AddPrefabFromPackage("Panel");
    }

    [MenuItem(toolRoot + "Panel Group", false, 1)]
    [MenuItem(menuRoot + "Panel Group", false, 1)]
    private static void PanelGroup(){
        AddPrefabFromPackage("Panel");
    }

    private static void AddPrefabFromPackage(string prefabName){
        // Build the full path to the prefab in the package
        string fullPath = PackagePrefabPath + prefabName + ".prefab";

        // Load the prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);

        if (prefab == null){
            Debug.LogError($"Prefab '{prefabName}' not found at '{fullPath}'");
            return;
        }

        // Instantiate the prefab in the scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (instance != null){
            Undo.RegisterCreatedObjectUndo(instance, $"Create {prefabName}");
            Selection.activeGameObject = instance; // Select the newly created object
            Debug.Log($"Prefab '{prefabName}' added to the scene from the package.");
        }else{
            Debug.LogError($"Failed to instantiate prefab '{prefabName}'.");
        }
    }
}

}
