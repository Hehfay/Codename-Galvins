using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : MonoBehaviour {
    private static Calendar instance;


    private float timeElapsed;
    public float startTime;
    public float secondsPerDay;
    public float daysPerYear;
	// Use this for initialization
	void Awake () {
        timeElapsed = startTime;
		if (instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
        timeElapsed += Time.deltaTime;
	}

    public static Calendar getInstance() {
        if (instance == null) {
            GameObject obj = new GameObject("Calendar");
            instance = obj.AddComponent<Calendar>();
        }
        return instance;
    }

    public float getTime() {
        return timeElapsed;
    }

    // TODO: fix all of below functions
    public float getTimeOfDay() {
        return 8;
    }

    public int getMonthOfYear() {
        return 12;
    }

    public int getYear () {
        return 2017;
    }

    public int getDayOfWeek () {
        return 1;
    }

}
