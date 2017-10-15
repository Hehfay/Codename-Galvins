using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PlusButtonOnClick : MonoBehaviour, IPointerClickHandler {

    Text displayCountText;

    public int maxCount;

    int currentCount;

    void Start () {
        GameObject CountDisplay = transform.parent.gameObject;
        displayCountText = CountDisplay.GetComponentInChildren<Text> ();
        Debug.Assert (displayCountText != null);
    }

    public void OnPointerClick (PointerEventData eventData) {
        currentCount = Convert.ToInt32(displayCountText.text);
        currentCount++;
        if (currentCount > maxCount) {
            currentCount = maxCount;
        }
        displayCountText.text = currentCount.ToString ();
    }
}
