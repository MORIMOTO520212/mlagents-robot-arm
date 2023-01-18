using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GameObject axis;

    void Start()
    {
        axis = this.transform.Find("Axis1").gameObject;
        HingeJoint axisJoint = axis.GetComponent<HingeJoint>();
        Debug.Log( axisJoint.angle );
    }

    void Update()
    {
        //Debug.Log(joints[0].angle);
    }
}
