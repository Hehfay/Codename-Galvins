using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        string input = Input.inputString;
        if (!input.Equals("")) {
            Debug.Log(input);
        }
	}
}
