namespace Aarware.Core.Editor{
    
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(PanelController))]
    class PanelControllerEditor : Editor{

        PanelController myTarget => target as PanelController;
        bool instant = false;
        SerializedProperty group;

        public override void OnInspectorGUI() {
            group   = serializedObject.FindProperty("group");
            instant = GUILayout.Toggle(instant, "Instant");
            
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
                myTarget.Open(instant);
            }else{
                ((CanvasGroup)group.objectReferenceValue).alpha=1f;
                serializedObject.ApplyModifiedProperties();
            }
        }
        void Close(){
            if(Application.isPlaying){
                myTarget.Close(instant);
            }else{
                ((CanvasGroup)group.objectReferenceValue).alpha=0f;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}