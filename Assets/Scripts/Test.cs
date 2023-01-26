using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public partial class Test : MonoBehaviour
{
    public Transform target;      // ターゲットオブジェクト
    public Transform endEffector; // エンドエフェクタオブジェクト
    public Transform axis1;
    public Transform axis2;

    void Start()
    {
        //axis1.Rotate(0, 0, 50);
    }

    void Update()
    {
    }

}
