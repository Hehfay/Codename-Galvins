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

        float time = calendar.getTime(); // eventually replace with game time (takes pauses, etc into effect)
        Quaternion finalRotation = getSunRotationAtTime(time);
        this.transform.rotation = finalRotation;
    }

    /**
     * Returns value [0, 1]
     */
    private float calculateSunIntensity (float angle, float maxIntensity = 1.0f) {
        float intensity = 0.0f;
        if (angle > 180 + angleOffset && angle < 360 - angleOffset) {
            intensity = 0;
        }
        else {
            float rad = MathUtil.convertDegreeToRad(angle);
            float minSinValue = Mathf.Sin(MathUtil.convertDegreeToRad(180 + angleOffset));
            float conversionConstant = 1 / (1 - Mathf.Sin(minSinValue));
            intensity = (Mathf.Sin(rad) - minSinValue) * conversionConstant * maxIntensity;
            Mathf.Clamp(intensity, 0.0f, maxIntensity);
        }
        intensity = 0.5f;

        return intensity;
    }

    private Quaternion getSunRotationAtTime (float time) {
        // first do earth spin
       

        float spinDegrees = earthSpinDegreesPerSecond * time;
        spinDegrees %= 360f;

        float revolutionDegrees = earthRevolutionDegreesPerSecond * time;
        revolutionDegrees %= 360f;

        spinDegrees -= revolutionDegrees; // have to minus the revolution because it will also spin as well.

        light.intensity = calculateSunIntensity(spinDegrees);

        // we're going to apply this one first, so even though the axis should be tilted, since its first we can just do forward;
        Quaternion spinRotation = Quaternion.AngleAxis(spinDegrees, Vector3.forward);

        Quaternion axisTilt = Quaternion.AngleAxis(axisOffset, Vector3.right);

        //Quaternion latitudeTilt = Quaternion.AngleAxis(latitude);

        Quaternion revolutionRotation = Quaternion.AngleAxis(revolutionDegrees, Vector3.forward);

        // By ordering the application of the rotations correctly, we made the axes above easier to use.
        return revolutionRotation * axisTilt * spinRotation;
    }
}
