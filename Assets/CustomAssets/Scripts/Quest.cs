using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType {
    Fetch
};

public class Quest : MonoBehaviour {

    bool started = false;
    
    int index = 0;

    public string[] text;

    public PickupData[] winCondition;

    public void Advance () {
        if (++index >= text.Length) {
            --index;
        }
    }

    public string GetDialog () {
        return text[index];
    }

    public bool ConditionMet (PickupData[] characterInventory) {

        if (!started) {
            started = true;
            return false;
        }

        for (int i = 0; i < characterInventory.Length; ++i) {
            if (characterInventory[i] == winCondition[index]) {
                return true;
            }
        }
        return false;
    }
}
