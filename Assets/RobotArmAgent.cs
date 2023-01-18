using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotArmAgent : Agent
{
    public int force; // アームの力
    public Transform target; // ターゲットオブジェクト
    public Transform endEffector; // エンドエフェクタオブジェクト

    private GameObject[] axis = new GameObject[2];
    

    // 1度限りの実行
    public override void Initialize()
    {
        // 軸オブジェクトの取得
        axis[0] = this.transform.Find("Axis1").gameObject;
        axis[1] = axis[0].transform.Find("Axis2").gameObject;
    }

    // 1エピソード限りの実行
    public override void OnEpisodeBegin()
    {
        foreach (GameObject ax in axis)
        {
            // アームを垂直に戻す

            // 力を0にする
            JointMotor motor = ax.GetComponent<HingeJoint>().motor;
            motor.force = 500;
            motor.targetVelocity = 0;
            ax.GetComponent<HingeJoint>().motor = motor; // 設定を代入
        }
    }

    // エージェントのベクトル観測を追加する
    public override void CollectObservations(VectorSensor sensor) // VectorSensorクラス
    {

    }

    // 提供されたアクションに基づいて、エージェントの動作を指定する
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float setForce = 100;

        // アームを動かす
        for (int i = 0; i < actionBuffers.ContinuousActions.Length; i++)
        {
            JointMotor motor = axis[i].GetComponent<HingeJoint>().motor;
            motor.targetVelocity = actionBuffers.ContinuousActions[i] * setForce;
            axis[i].GetComponent<HingeJoint>().motor = motor; // 設定を代入
        }

        // 距離を計測
        float distanceToTarget = Vector3.Distance(endEffector.localPosition, target.localPosition);
        if (distanceToTarget < 1f)
        {

        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
