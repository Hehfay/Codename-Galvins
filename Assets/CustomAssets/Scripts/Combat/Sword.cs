using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {
    public float damage = 100.0f;
    Animator animator;
    Collider swordCol;
    bool collisionsActive;
	// Use this for initialization
	void Awake () {
        swordCol = GetComponent<MeshCollider>();
        collisionsActive = false;
        SetActivateCollisions(collisionsActive);
        animator = transform.root.GetComponent<Animator>(); // get the human's animator
        //make sure that the sword collider does not collide with the person holding it
        Collider[] bodyCols = transform.root.GetComponentsInChildren<Collider>(); // gets all colliders attached to this person
        foreach (Collider col in bodyCols) {
            Physics.IgnoreCollision(swordCol, col);
        }
	}
	
	// Update is called once per frame
	void Update () {
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("2HAttack")) {
            float playbackTime = animInfo.normalizedTime % 1.0f;
            if (!collisionsActive && playbackTime > 0.5f && playbackTime < 0.75f) {
                SetActivateCollisions(true); // collisions active
            } else if (collisionsActive && (playbackTime <= 0.50 || playbackTime >= 0.75)) {
                SetActivateCollisions(false); // collisions not active
            }

        }
	}

    void OnCollisionEnter(Collision collision) {
        DamageExampleTest d = collision.gameObject.GetComponent<DamageExampleTest>();
        if (d != null) {
            DamageTest dt = new DamageTest();
            dt.damageValue = 100;
            d.Damage(dt);
        }
    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log(collider.gameObject.name);
        DamageExampleTest d = collider.gameObject.GetComponent<DamageExampleTest>();
        if (d != null) {
            DamageTest dt = new DamageTest();
            dt.damageValue = damage;
            d.Damage(dt);
        }
    }

    public void Swing () {
        // play animation
    }

    private void SetActivateCollisions (bool active) {
        swordCol.enabled = active;
        swordCol.isTrigger = active; // this is a workaround for funky unity behavior -> if collider is trigger, it still triggers when not active
    }
}
