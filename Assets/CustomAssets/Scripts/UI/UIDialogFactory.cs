using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogFactory : MonoBehaviour {
    public GameObject UIDialog;
    public GameObject UIDialogText;
    public GameObject DialogOptionButton;

    private List<GameObject> references;
    private GameObject Canvas;

    void Start () {
        Canvas = GameObject.Find ("Canvas");
        references = new List<GameObject> ();
        GetComponent<UIDialogFactory> ().enabled = false;
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.F)) {
            GameObject PlayerReference = Canvas.transform.root.GetComponent<PlayerReferenceContainer> ().Player;
            PlayerReference.GetComponent<CursorManager> ().enabled = true;
            PlayerReference.GetComponent<PlayerMovementController> ().enabled = true;
            PlayerReference.GetComponent<CursorManager> ().LockCursor ();
            PlayerReference.GetComponent<CursorManager> ().enabled = true;
            PlayerReference.GetComponent<ColliderInteractController> ().enabled = true;
            PlayerReference.GetComponent<UIInputHandler> ().enabled = true;
            PlayerReference.GetComponent<UITabsFactory> ().enabled = true;
            DestroyFactoryItem ();
            enabled = false;
        }
    }

    public void CreateFactoryItem () {
        if (references == null) {
            references = new List<GameObject>();
        }
        if (Canvas == null) {
            Canvas = GameObject.Find ("Canvas");
        }
        references.Clear ();
        references.Add (Instantiate(UIDialog, Canvas.transform, false));

        // Populate the dialog box with dialog.
        AIDialogContainer aiDialogContainer = GetComponent<AIDialogContainer> ();
        for (int i = 0; i < aiDialogContainer.dialogTopics.Length; ++i) {
            GameObject dialogOption = Instantiate (DialogOptionButton, references[0].transform.GetChild (1).transform.GetChild(0).transform.GetChild(0), false);
            dialogOption.GetComponentInChildren<Text> ().text = aiDialogContainer.dialogTopics[i];

            dialogOption.GetComponent<DialogTopicOnClick> ().dialogTopic = GetComponent<AIDialogContainer> ().dialogTopics[i];
            dialogOption.GetComponent<DialogTopicOnClick> ().dialog = GetComponent<AIDialogContainer> ().dialog[i];
            dialogOption.GetComponent<DialogTopicOnClick> ().questTrigger = GetComponent<AIDialogContainer> ().questTriggers[i];
        }

        // Create a UIDialogText with the greeting.
        // Select a random greeting.
        int greetingIndex = Mathf.RoundToInt(Random.Range (0, GetComponent<AIDialogContainer> ().greeting.Length-1));

        GameObject greeting = Instantiate (UIDialogText, references[0].transform.GetChild(0).transform.GetChild(0).transform.GetChild(0), false);
        greeting.GetComponentInChildren<Text>().text = GetComponent<AIDialogContainer> ().greeting[greetingIndex];

        // greeting.transform.parent.GetComponent<RectTransform> ().he += greeting.GetComponent<RectTransform> ().rect.height;
    }

    private void DestroyFactoryItem () {
        if (references.Count == 0) {
            return;
        }
        Destroy (references[0]);
        references.Clear ();
    }
}