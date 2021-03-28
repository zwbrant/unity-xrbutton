using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XRButton))]
public class XRButtonEditor : XRBaseInteractableEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(interactionLayerMask);
        EditorGUILayout.LabelField("SDA");
        serializedObject.ApplyModifiedProperties();
    }
}
