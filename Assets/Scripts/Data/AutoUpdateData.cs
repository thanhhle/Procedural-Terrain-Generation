using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUpdateData : ScriptableObject 
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyUpdates;
        }
    }

    public void NotifyUpdates()
    {
        UnityEditor.EditorApplication.update -= NotifyUpdates;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
