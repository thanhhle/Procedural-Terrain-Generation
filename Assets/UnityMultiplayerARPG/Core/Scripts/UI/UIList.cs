﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIList : MonoBehaviour
{
    public GameObject uiPrefab;
    public Transform uiContainer;
    public bool doNotRemoveContainerChildren;
    public IEnumerable List { get; protected set; }
    public int ListCount { get; protected set; } = 0;
    public System.Action<int, object, GameObject> onGenerateEntry = null;
    protected readonly List<GameObject> uis = new List<GameObject>();
    protected bool removedContainerChildren;

    public void RemoveContainerChildren()
    {
        if (removedContainerChildren || doNotRemoveContainerChildren)
            return;
        removedContainerChildren = true;
        uiContainer.RemoveChildren();
    }

    public void Generate<T>(IEnumerable<T> list, System.Action<int, T, GameObject> onGenerateEntry)
    {
        RemoveContainerChildren();

        List = list;
        int i = 0;
        foreach (T entry in list)
        {
            // NOTE: `ui` can be NULL
            GameObject ui = null;
            if (i < uis.Count)
            {
                ui = uis[i];
                ui.SetActive(true);
            }
            else
            {
                if (uiPrefab != null && uiContainer != null)
                {
                    ui = Instantiate(uiPrefab);
                    ui.transform.SetParent(uiContainer);
                    ui.transform.localPosition = Vector3.zero;
                    ui.transform.localRotation = Quaternion.identity;
                    ui.transform.localScale = Vector3.one;
                    ui.transform.SetAsLastSibling();
                    uis.Add(ui);
                    ui.SetActive(true);
                }
            }
            if (this.onGenerateEntry != null)
                this.onGenerateEntry.Invoke(i, entry, ui);
            if (onGenerateEntry != null)
                onGenerateEntry.Invoke(i, entry, ui);
            ++i;
        }
        ListCount = i;
        for (; i < uis.Count; ++i)
        {
            GameObject ui = uis[i];
            ui.SetActive(false);
        }
    }

    public void HideAll()
    {
        RemoveContainerChildren();

        for (int i = 0; i < uis.Count; ++i)
        {
            GameObject ui = uis[i];
            ui.SetActive(false);
        }
    }
}
