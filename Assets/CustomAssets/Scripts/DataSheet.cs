using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DataSheet : ScriptableObject {
    // The number of hands this combat equipment takes.
    public HandOccupancies handOccupancy;

    // Is this combat equipment armor? Armor is handled particularly; it cannot be put in weapon slots.
    public bool isArmor;

    // Public facing name of the equipment.
    public string equipmentName;

    // Description of the equipment.
    public string description;

    public string hoverText;

    // If stackable then there may be more than one in an inventory slot.
    public bool stackable;

    public float damage;


    public QuestTrigger questTrigger;
    public QuestUnTrigger questUnTrigger;

    public string MeshFilterName;
    public string MaterialName;

    // TODO All the other combat equipment stuff.
    // public Mesh mesh;
    // public SwingAnimation swinganimation;
    // public Sound sound.

    // public MeshFilter meshFilter;
    // public BoxCollider boxCollider;
    // public MeshRenderer meshRenderer;
    // public Rigidbody rigidBody;
}
