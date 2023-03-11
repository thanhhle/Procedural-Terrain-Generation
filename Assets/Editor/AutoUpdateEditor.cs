using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoUpdate), true)]
public class AutoUpdateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AutoUpdate data = (AutoUpdate)target;

        if (GUILayout.Button("Update"))
        {
            data.NotifyUpdates();
            EditorUtility.SetDirty(target);
        }
    }
}
