using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSocket : MonoBehaviour {

    [SerializeField] // makes it editable in editor
    EquipSlotType _equipSlotType;
    public EquipSlotType equipSlotType { get { return _equipSlotType; } } // public accessor during runtime

    private bool isFilled;
    private GameObject equipedObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetEquipedObject() {
        return equipedObject;
    }

    public bool EquipToSocket(GameObject itemToEquip) {
        Component comp = itemToEquip.GetComponent( typeof(IEquipable) );
        IEquipable equip = comp as IEquipable;
        if (comp && !isFilled && equip.GetEquipSlotType() == equipSlotType) {
            UnequipSocket();
            equipedObject = itemToEquip;
            isFilled = true;
            // TODO: use equipSlotType to map skeletal stuff to model of actor so that it animates correctly
        }
        return isFilled;
    }

    public void UnequipSocket() {
        equipedObject = null;
        isFilled = false;
    }
}
