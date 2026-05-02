using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;


public class BagSlotView : MonoBehaviour
{
    public int SlotIndex;
    public GameObject HighlightMask;

public void SetHighlight(bool on)
    {
        if (HighlightMask == null) return;

        HighlightMask.SetActive(on);

        if (on)
            HighlightMask.transform.SetAsLastSibling();
    }
}