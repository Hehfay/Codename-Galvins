using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {


    public float angleOffset = 20.0f;
    public float axisOffset = -23.5f;
    public float latitude = 70f;

    private float earthSpinDegreesPerSecond;
    private float earthRevolutionDegreesPerSecond;

    private Calendar calendar;
    private Light light;

    private int numDaysSinceStart = 0;

    // Use this for initialization
    void Start () {
        calendar = Calendar.getInstance();
        earthSpinDegreesPerSecond = 360 / calendar.secondsPerDay; // how many degrees per second
        earthRevolutionDegreesPerSecond = 360 / (calendar.daysPerYear * calendar.secondsPerDay); // how much revolution per second
        //this.transform.rotation = Quaternion.Euler(new Vector3(axisOffset, 0f, 0f));
        light = GetComponentInChildren<Light>();
    }
	
	// Update is called once per frame
	void Update () {
        earthSpinDegreesPerSecond = 360 / calendar.secondsPerDay; // how many degrees per second
        earthRevolutionDegreesPerSecond = 360 / (calendar.daysPerYear * calendar.secondsPerDay); // how much revolution per second

        float time = calendar.getTime();
        Quaternion finalRotation = getSunRotationAtTime(time);
        this.transform.rotation = finalRotation;

        light.intensity = calculateSunIntensity();
    }

    /**
     * Returns value [0, 1]
     */
    private float calculateSunIntensity (float maxIntensity = 1.0f) {
        float intensity = 0.0f;
        float dot = Vector3.Dot(light.transform.forward.normalized, Vector3.up);
        float rad = Mathf.PI - Mathf.Acos(dot);
        float angle = MathUtil.convertRadToDegree(rad);
        if (angle <= 90) {
            intensity = maxIntensity;
        } else if (angle <= (90 + angleOffset)) {
            float t = Mathf.InverseLerp(90 + angleOffset, 90, angle);
            intensity = t;
        }
        return intensity;
    }

    private Quaternion getSunRotationAtTime (float time) {
        // first do earth spin
       

        float spinDegrees = earthSpinDegreesPerSecond * time;
        spinDegrees += 180; // this is so that time 0 in game time results in 12 am sun time
        spinDegrees %= 360f;

        float revolutionDegrees = earthRevolutionDegreesPerSecond * time;
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("down");
        }
        revolutionDegrees %= 360f;

        

        Quaternion earthAxisRelativeToGroundAtLatitude = Quaternion.AngleAxis(-latitude, Vector3.right);

        Quaternion axisTilt = Quaternion.AngleAxis(axisOffset, Vector3.right);
        Debug.DrawRay(transform.position, axisTilt * Vector3.forward, Color.blue);

        Quaternion sunAxisRotationAboutEarthAxisRelToGround = Quaternion.AngleAxis(spinDegrees + revolutionDegrees, earthAxisRelativeToGroundAtLatitude * Vector3.forward);

        // apply the rotations
        Quaternion finalAxis = sunAxisRotationAboutEarthAxisRelToGround * axisTilt * earthAxisRelativeToGroundAtLatitude;

        // undo seasonal rotation of sun affecting day night cycle of sun.
        finalAxis = Quaternion.AngleAxis(-revolutionDegrees, finalAxis * Vector3.forward) * finalAxis;
        return finalAxis;
    }
}
