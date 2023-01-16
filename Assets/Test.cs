using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private HingeJoint[] joints; // クラス<型>

    void Start()
    {
        joints = GetComponentsInChildren<HingeJoint>(); // クラス<型>
    }

    void Update()
    {
        Debug.Log(joints[0].angle);
    }
}
