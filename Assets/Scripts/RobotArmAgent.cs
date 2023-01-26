using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public partial class RobotArmAgent : Agent
{
    public bool isTraining;
    public Transform target;      // ターゲットオブジェクト
    public Transform endEffector; // エンドエフェクタオブジェクト

    private GameObject[] axis = new GameObject[2]; // 軸
    private Vector3[] targetPositionList;
    private float prevBest;

    // 1度限りの実行
    public override void Initialize()
    {
        targetPositionList = new [] {
            new Vector3(0.429f, 1.172f, 0f),
            new Vector3(-0.759f, 1.172f, 0f),
            /*new Vector3(0.096f, 0.636f, 0f),
            new Vector3(0.861f, -0.783f, 0f),
            new Vector3(1.288f, -0.25f, 0f),
            new Vector3(1.288f, 0.358f, 0f),
            new Vector3(-1.625f, 0.358f, 0f),
            new Vector3(0.778f, -0.77f, 0f),
            new Vector3(-1.129f, -0.503f, 0f),
            new Vector3(-1.17f, 0.883f, 0f),
            new Vector3(-0.617f, 0.5f, 0f),*/
        };

        // 軸オブジェクトの取得
        axis[0] = this.transform.Find("Axis1").gameObject;
        axis[1] = axis[0].transform.Find("Axis2").gameObject;
        // useMotor
        axis[0].GetComponent<HingeJoint>().useMotor = false;
        axis[1].GetComponent<HingeJoint>().useMotor = false;
        // useLimits
        axis[0].GetComponent<HingeJoint>().useLimits = false;
        axis[1].GetComponent<HingeJoint>().useLimits = false;
    }

    // 1エピソード限りの実行
    public override void OnEpisodeBegin()
    {
        foreach (GameObject ax in axis)
        {
            // アームを垂直に戻す
            Vector3 localAngle = ax.transform.localEulerAngles;
            localAngle.z = 0;
            ax.transform.localEulerAngles = localAngle;
        }
        target.localPosition = targetPositionList[Random.Range(0, targetPositionList.Length)];

        // 前の距離prevBestの初期化
        prevBest = Vector3.Distance(endEffector.position, target.position);
    }

    // エージェントのベクトル観測を追加する
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.position);            // ターゲットの位置
        sensor.AddObservation(endEffector.position);       // エンドエフェクタの位置
        sensor.AddObservation(axis[0].transform.localEulerAngles.z); // 軸の角度
        sensor.AddObservation(axis[1].transform.localEulerAngles.z); // 軸の角度
        sensor.AddObservation(Vector3.Distance(endEffector.position, target.position));
    }

    // 提供されたアクションに基づいて、エージェントの動作を指定する
    public override void OnActionReceived(ActionBuffers actions)
    {
        float beginDistance = Vector3.Distance(endEffector.position, target.position);

        // アームを動かす
        for (int i = 0; i < 2; i++)
        {  
            switch (actions.DiscreteActions[i])
            {
                case 0:
                    axis[i].transform.Rotate(0, 0, -1);
                    break;
                case 1:
                    axis[i].transform.Rotate(0, 0, 1);
                    break;
                default:
                    break;
            }
        }
        //Debug.Log(actions.DiscreteActions[0] + "  " + actions.DiscreteActions[1]);

        // ターゲットに接触したらプラス報酬
        float distance = Vector3.Distance(endEffector.position, target.position);

        // 以前の距離に近づくとマイナス報酬が小さくなっていく
        if (distance > prevBest)
        {
            AddReward(prevBest - distance);
        }
        // 以前の距離より近くなるとプラス報酬
        else
        {
            float distanceDiff = beginDistance - distance;
            AddReward(distanceDiff);
            prevBest = distance;
        }

        // ターゲットに到達したらボーナス報酬
        if (distance < 0.3f)
        {
            //AddReward(0.6f);
        }

        // 床面に当たった場合、ペナルティ
        if (endEffector.position.y < 0.3)
        {
            AddReward(-1f);
            EndEpisode();
        }

        AddReward(-0.0001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    void Update()
    {
    }
}
