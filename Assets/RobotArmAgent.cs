using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class RobotArmAgent : Agent
{
    public Transform target;      // ターゲットオブジェクト
    public Transform endEffector; // エンドエフェクタオブジェクト
    private GameObject[] axis = new GameObject[2];
    private Vector3[] targetPositionList;

    // 1度限りの実行
    public override void Initialize()
    {
        targetPositionList = new [] {
            new Vector3(0.096f, 0.636f, 0f),
            new Vector3(0.861f, -0.783f, 0f),
            new Vector3(1.288f, -0.25f, 0f),
            new Vector3(1.288f, 0.358f, 0f),
            new Vector3(-1.625f, 0.358f, 0f),
            new Vector3(0.778f, -0.77f, 0f),
            new Vector3(-1.129f, -0.503f, 0f),
            new Vector3(-1.17f, 0.883f, 0f),
            new Vector3(-0.617f, 0.5f, 0f),
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
    }

    // エージェントのベクトル観測を追加する
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);            // ターゲットの位置
        sensor.AddObservation(endEffector.localPosition);       // エンドエフェクタの位置
        sensor.AddObservation(axis[0].transform.localEulerAngles.z); // 軸の角度
        sensor.AddObservation(axis[1].transform.localEulerAngles.z); // 軸の角度
        sensor.AddObservation(Vector3.Distance(endEffector.position, target.position));
    }

    // 提供されたアクションに基づいて、エージェントの動作を指定する
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // アームを動かす
        for (int i = 0; i < 2; i++)
        {
            axis[i].transform.Rotate(0, 0, actionBuffers.ContinuousActions[i]);
        }

        // ターゲットに接触したらプラス報酬
        float distanceToTarget = Vector3.Distance(endEffector.position, target.position);
 
        if (distanceToTarget < 0.3f)
        {
            Debug.Log("goal");
            AddReward(1.0f);
            EndEpisode();
        }
        else if (distanceToTarget < 1.0f)
        {
            AddReward(0.5f);
        }

        // 関節の角度が閾値外であればマイナス報酬
        // 閾値:-110~110°
        foreach (GameObject ax in axis)
        {
            if (
                !((-110 <= ax.transform.localEulerAngles.z && ax.transform.localEulerAngles.z <= 110) || 
                (250 <= ax.transform.localEulerAngles.z && ax.transform.localEulerAngles.z <= 360))
                )
            {
                AddReward(-1.0f);
                EndEpisode();
            }
        }
        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
