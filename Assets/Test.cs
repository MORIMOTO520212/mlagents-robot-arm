using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private HingeJoint[] joints; // クラス<型>

    void Start()
    {
        joints = GetComponentsInChildren<HingeJoint>(); // クラス<型>

        JointMotor motor0 = joints[0].motor;
        JointMotor motor1 = joints[1].motor;

        motor0.force = 100;
        motor0.targetVelocity = 60;
        joints[0].motor = motor0;
    }

    void Update()
    {
        //Debug.Log(joints[0].angle);
    }
}
