namespace Aarware.Core.Editor{
    
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Script:  PanelControllerEditor.cs
    /// Created: 6/13/2022 10:49:03 AM
    /// Author:  AaronBuffie
    /// </summary>

    [CustomEditor(typeof(PanelController))]
    class PanelControllerEditor : Editor{

        PanelController myTarget => target as PanelController;
       // bool instant = false;
        SerializedProperty group;

        public override void OnInspectorGUI() {
            group   = serializedObject.FindProperty("group");
           // instant = GUILayout.Toggle(instant, "Instant");
            
            GUILayout.BeginHorizontal();
            if(myTarget.Closed){
                if(GUILayout.Button("Open")){Open();}
            }else{                
                if(GUILayout.Button("Close")){Close();}
            }
            GUILayout.EndHorizontal();

            DrawDefaultInspector();
        }

        void Open(){
            if(Application.isPlaying){
                myTarget.Open();
            }else{
                ((CanvasGroup)group.objectReferenceValue).alpha=1f;
                serializedObject.ApplyModifiedProperties();
            }
        }
        void Close(){
            if(Application.isPlaying){
                myTarget.Close();
            }else{
                ((CanvasGroup)group.objectReferenceValue).alpha=0f;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}