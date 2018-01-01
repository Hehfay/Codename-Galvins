using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestDoSmithing : MonoBehaviour {

    private Calendar cal;
    Task activeTask;
    public Task[] dailyTasks;
    NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        cal = Calendar.getInstance();
        cal.getDayOfWeek();
        activeTask = null;
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        float timeOfDay = cal.getTimeOfDay();
        int dayOfWeek = cal.getDayOfWeek();
		foreach (Task task in dailyTasks) {
            if (task.timeBegin <= timeOfDay && task.timeEnd > timeOfDay && task.days[dayOfWeek] == 1) {
                activeTask = task;
                break;
            }
        }
        if (activeTask != null) {
            if (activeTask.timeEnd > timeOfDay) {
                agent.SetDestination(activeTask.taskLocation.position);
            } else {
                activeTask = null;
            }
        }
	}
}
