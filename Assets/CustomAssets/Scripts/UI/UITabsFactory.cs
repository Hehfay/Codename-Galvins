using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UITabsFactory : MonoBehaviour {
    public GameObject UITabs;
    List<GameObject> references;
    private GameObject Canvas;

    void Start () {
        if (!(GetComponent<NetworkIdentity> ().isLocalPlayer)) {
            return;
        }
        references = new List<GameObject>();
        Canvas = GameObject.Find ("Canvas");
        Canvas.GetComponent<PlayerReferenceContainer> ().Player = gameObject;
    }

    public void CreateFactoryItem () {
        references.Clear ();
        references.Add (Instantiate(UITabs, Canvas.transform, false));
    }
}
