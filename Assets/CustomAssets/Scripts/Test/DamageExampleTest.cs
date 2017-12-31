using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageExampleTest : MonoBehaviour, IDamageable<DamageTest>, IKillable {

    // Use this for initialization
    public Animator animator;
    public float health;
    public float testDamageResistance;

	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Damage(DamageTest d) {
        health = health - ((1 - testDamageResistance) * d.damageValue);
        if (health <= 0.0f && this is IKillable) {
            Kill();         
        }
    }

    public void Kill () {
        int i = Random.Range(1, 3);
        animator.SetInteger("Death", i);
        // play kill animation / sound effect
        // make lootable by activating (instead of pickpocketing only)
        // make known to game logic that person is killed if discovered or missed somewhere they should be
        // possibly make avatar disappear after some amount of time
    }
}
