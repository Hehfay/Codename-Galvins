using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ActorAI : MonoBehaviour {

    public AIPackage generalAI;
    NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(generalAI.getTarget());
    }
	
	// Update is called once per frame
	void Update () {
        
        RaycastHit hit;
        Ray ray = new Ray(agent.transform.position, agent.nextPosition - agent.transform.position);
        bool collision = Physics.Raycast(ray, out hit, 3 * agent.radius);
        if (collision) {
            Debug.Log("I am finding my way around");
        }
	}

    public void doInteract() {
        Debug.Log("Target = " + agent.destination);
        Debug.Log("I am at :" + transform.position);
    }
}
