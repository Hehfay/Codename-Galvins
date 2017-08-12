using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour {
    int index = 0;
    public string[] text;

    public void Advance () {
        ++index;
    }

    public string GetDialog () {
        return text[index];
    }

}
