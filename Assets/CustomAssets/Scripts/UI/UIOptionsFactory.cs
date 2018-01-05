using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionsFactory : MonoBehaviour {
    public GameObject UIOptionsMenu;

    private List<GameObject> references;
    private GameObject Canvas;

    void Start () {
        references = new List<GameObject> ();
        Canvas = GameObject.Find ("Canvas");
    }

    public void CreateFactoryItem () {
        references.Clear ();
        references.Add (Instantiate(UIOptionsMenu, Canvas.transform, false));

        // Get rememberUIState
        references[0].transform.GetChild (0).transform.GetChild (0).transform.GetChild (0).GetComponent<Toggle> ().isOn =
            GetComponent<PlayerSettings> ().rememberUIState;

        // Get framerate
        references[0].transform.GetChild (0).transform.GetChild (0).transform.GetChild (1).GetComponent<InputField> ().text =
            GetComponent<PlayerSettings> ().framerate.ToString();
    }

    public void DestroyFactoryItem () {
        if (references.Count == 0) {
            return;
        }
        Destroy (references[0]);
        references.Clear ();
    }
}
