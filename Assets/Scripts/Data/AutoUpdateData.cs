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
            NotifyUpdates();
        }
    }

    public void NotifyUpdates()
    {
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
