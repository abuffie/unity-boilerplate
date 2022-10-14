namespace Aarware.Core.Editor{
   
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.SceneManagement;

    [CustomEditor(typeof(SceneController))]
    class SceneControllerEditor : Editor {
        
        int scene=0;
        string[] scenes;
        SceneController myTarget => (SceneController)target;

        void OnEnable() {
            scenes = EditorBuildSettings.scenes.Select(s=> Path.GetFileNameWithoutExtension(s.path)).ToArray();
        }
        public override void OnInspectorGUI() {
            GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
            GUI.color           = Application.isPlaying ? Color.white : Color.yellow;
            GUILayout.BeginHorizontal("Box");
                EditorGUI.BeginDisabledGroup(!Application.isPlaying);

                if(!Application.isPlaying){GUILayout.Label("Playmode Only");}
                scene = EditorGUILayout.Popup(scene, scenes  );
                if(GUILayout.Button("Load")){
                    myTarget.GoToScene(scenes[scene]);
                }
                EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            GUI.color           = Color.white;

            DrawDefaultInspector();
            
        }
    }
}