using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabsFactory : MonoBehaviour {
    public GameObject UITabs;
    List<GameObject> references;
    private GameObject Canvas;

    void Start () {
        references = new List<GameObject>();
        Canvas = GameObject.Find ("Canvas");
        Canvas.GetComponent<PlayerReferenceContainer> ().Player = gameObject;
    }

    public void CreateFactoryItem () {
        references.Clear ();
        references.Add (Instantiate(UITabs, Canvas.transform, false));
    }
}
