using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestLogFactory : MonoBehaviour {
    public GameObject UIQuestLog;

    private List<GameObject> references;
    private GameObject Canvas;

	// Use this for initialization
	void Start () {
        references = new List<GameObject> ();
        Canvas = GameObject.Find ("Canvas");
	}

    public void CreateFactoryItem () {
        references.Clear ();
        references.Add (Instantiate(UIQuestLog, Canvas.transform, false));
        GameObject.Find ("QuestManager").GetComponent<QuestManager> ().WriteToQuestLog ();
    }

    public void DestroyFactoryItem () {
        if (references.Count == 0) {
            return;
        }
        Destroy (references[0]);
        references.Clear ();
    }
}
