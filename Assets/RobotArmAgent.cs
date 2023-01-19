using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotArmAgent : Agent
{
    public int force;             // アームの力
    public Transform target;      // ターゲットオブジェクト
    public Transform endEffector; // エンドエフェクタオブジェクト

    private GameObject[] axis = new GameObject[2];
    

    // 1度限りの実行
    public override void Initialize()
    {
        // 軸オブジェクトの取得
        axis[0] = this.transform.Find("Axis1").gameObject;
        axis[1] = axis[0].transform.Find("Axis2").gameObject;
        // useMotor
        axis[0].GetComponent<HingeJoint>().useMotor = true;
        axis[1].GetComponent<HingeJoint>().useMotor = true;
        // useLimits
        axis[0].GetComponent<HingeJoint>().useLimits = true;
        axis[1].GetComponent<HingeJoint>().useLimits = true;
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

            // 力を0にする
            JointMotor motor = ax.GetComponent<HingeJoint>().motor;
            motor.force = 500;
            motor.targetVelocity = 0;
            
            // limitsを初期化
            JointLimits limits = ax.GetComponent<HingeJoint>().limits;
            limits.min = 0;
            limits.max = 0;

            // 設定を代入
            ax.GetComponent<HingeJoint>().motor = motor;
            ax.GetComponent<HingeJoint>().limits = limits;
        }
    }

    // エージェントのベクトル観測を追加する
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);            // ターゲットの位置
        sensor.AddObservation(endEffector.localPosition);       // エンドエフェクタの位置
        sensor.AddObservation(axis[0].transform.localEulerAngles.z); // 軸の角度
        sensor.AddObservation(axis[1].transform.localEulerAngles.z); // 軸の角度
    }

    // 提供されたアクションに基づいて、エージェントの動作を指定する
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float setForce = 50;

        // アームを動かす
        for (int i = 0; i < 2; i++)
        {
            JointMotor motor = axis[i].GetComponent<HingeJoint>().motor;
            JointLimits limits = axis[i].GetComponent<HingeJoint>().limits;

            // ジョイントの回転方向
            if (actionBuffers.DiscreteActions[i] == 0) // 右回転
            {
                motor.targetVelocity = setForce * -1;
                limits.min = actionBuffers.ContinuousActions[i] * -100;
            }
            else if (actionBuffers.DiscreteActions[i] == 1) // 左回転
            {
                motor.targetVelocity = setForce * 1;
                limits.max = actionBuffers.ContinuousActions[i] * 100;
            }

            // 設定を代入
            axis[i].GetComponent<HingeJoint>().motor = motor;
            axis[i].GetComponent<HingeJoint>().limits = limits;
        }

        // ターゲットに接触したらプラス報酬
        float distanceToTarget = Vector3.Distance(endEffector.position, target.position);
 
        if (distanceToTarget < 0.3f)
        {
            Debug.Log("goal");
            AddReward(2.0f); // 報酬
            EndEpisode();
        }
        else if (distanceToTarget < 0.8f)
        {
            Debug.Log("near");
            AddReward(0.6f);
            EndEpisode();
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
                Debug.Log("threshold:" + ax.transform.localEulerAngles.z);
                AddReward(-1.0f);
                EndEpisode();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
