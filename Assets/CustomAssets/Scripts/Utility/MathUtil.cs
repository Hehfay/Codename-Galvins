using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil {

    public static float convertRadToDegree(float a) {
        return a * Mathf.Rad2Deg;
    }
    public static float convertDegreeToRad(float a) {
        return a * Mathf.Deg2Rad;
    }
}