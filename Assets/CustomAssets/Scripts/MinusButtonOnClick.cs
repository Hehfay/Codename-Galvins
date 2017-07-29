using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MinusButtonOnClick : MonoBehaviour, IPointerClickHandler {

    Text displayCountText;

    int currentCount;

    void Start () {
        GameObject CountDisplay = transform.parent.gameObject;
        displayCountText = CountDisplay.GetComponentInChildren<Text> ();
        Debug.Assert (displayCountText != null);
    }

    public void OnPointerClick (PointerEventData eventData) {
        currentCount = Convert.ToInt32(displayCountText.text);
        currentCount--;
        if (currentCount < 1) {
            currentCount = 1;
        }
        displayCountText.text = currentCount.ToString ();
    }
}
