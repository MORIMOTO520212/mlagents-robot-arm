using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform target;      // ターゲットオブジェクト
    public Transform endEffector; // エンドエフェクタオブジェクト

    void Start()
    {
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(endEffector.position, target.position);
        Debug.Log("distanceToTarget:" + distanceToTarget);
    }
}
