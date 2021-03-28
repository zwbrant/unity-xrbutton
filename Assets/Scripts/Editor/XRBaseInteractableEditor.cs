using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRBaseInteractable), true)]
public class XRBaseInteractableEditor : Editor
{
    public SerializedProperty interactionLayerMask;

    public void OnEnable()
    {
        interactionLayerMask = serializedObject.FindProperty("interactionLayerMask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(interactionLayerMask);
        serializedObject.ApplyModifiedProperties();
    }
}
