namespace Aarware.Core.Editor{
    
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Script:  ConfimButtonEditor.cs
    /// Created: 6/13/2022 10:49:03 AM
    /// Author:  AaronBuffie
    /// </summary>
    [CustomEditor(typeof(ConfirmButton))]
    public class ConfirmButtonEditor : UnityEditor.UI.ButtonEditor {

        ConfirmButton       myTarget => (ConfirmButton)target;
        SerializedProperty  progressImage;
        SerializedProperty  enabledEffect;
        SerializedProperty  duration;
        SerializedProperty  progressColors;
        SerializedProperty  onRelease;
        SerializedProperty  disable;
        SerializedProperty  reEnable;
        SerializedProperty  reEnableWait;

        public override void OnInspectorGUI() {
            // make sure we have all the changes
            serializedObject.Update();

            // grab the ser ref for the class properties 
            progressImage   = serializedObject.FindProperty("progressImage");
            progressColors  = serializedObject.FindProperty("progressColors");
            enabledEffect   = serializedObject.FindProperty("enabledEffect");
            duration        = serializedObject.FindProperty("duration");
            onRelease       = serializedObject.FindProperty("onRelease");
            disable         = serializedObject.FindProperty("disable");
            reEnable        = serializedObject.FindProperty("reEnable");
            reEnableWait    = serializedObject.FindProperty("reEnableWait");

            // draw out the custom fields        
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Confirm Props", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(progressImage);
            EditorGUILayout.PropertyField(progressColors);
            EditorGUILayout.PropertyField(enabledEffect);
            EditorGUILayout.PropertyField(duration);     
            EditorGUILayout.PropertyField(onRelease);
            EditorGUILayout.PropertyField(disable);
            EditorGUILayout.PropertyField(reEnable);
            if(reEnable.boolValue){       
                EditorGUILayout.PropertyField(reEnableWait);
            }
            GUILayout.EndVertical();

            // Apply any changes
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(20);

            // draw the default button inspector
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Button Props", EditorStyles.boldLabel);
            base.OnInspectorGUI();
            GUILayout.EndVertical();
        }
            

    }
}