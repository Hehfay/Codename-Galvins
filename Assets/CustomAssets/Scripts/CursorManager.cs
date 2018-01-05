using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles locking and unlocking the cursor when 
// escape is pressed. It also handles loading the cursor texture.
public class CursorManager : MonoBehaviour {

    public bool cursorLocked;
    public TextAsset crossHairRaw;
    private Texture2D crossHair;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        cursorLocked = true;
        Cursor.visible = true;
        crossHair = new Texture2D (16, 16);
        crossHair.LoadImage (crossHairRaw.bytes);
	}
	
    public void UnlockCursor () {
        cursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor () {
        cursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnGUI () {
        if (cursorLocked) {
            float xMin;
            float yMin;
            if (cursorLocked) {
                xMin = (Screen.width / 2) - (crossHair.width / 2);
                yMin = ((Screen.height / 2) - (crossHair.height / 2) - 16);
            }
            else {
                xMin = Input.mousePosition.x - (crossHair.width / 2);
                yMin = (Screen.height - Input.mousePosition.y) - (crossHair.height / 2) + 3;
            }
            GUI.DrawTexture (new Rect (xMin, yMin, crossHair.width, crossHair.height), crossHair);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}