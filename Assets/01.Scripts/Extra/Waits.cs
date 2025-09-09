using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waits : MonoBehaviour
{
    private static Dictionary<float, WaitForSeconds> waits = new();
    public static  WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();

    public static WaitForSeconds GetWait(float t)
    {
        if(!waits.ContainsKey(t)) waits.Add(t, new WaitForSeconds(t));
        return waits[t];
    }

}
