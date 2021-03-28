using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;


[CustomEditor(typeof(SalmonScript))]
public class SalmonScriptEditor : Editor
{
    AnimBool m_ShowExtraFields;

    void OnEnable()
    {
        m_ShowExtraFields = new AnimBool(true);
        m_ShowExtraFields.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("bagel");

        if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("sadad");
            EditorGUILayout.PrefixLabel("Color");
        }
        serializedObject.ApplyModifiedProperties();
    }
}
