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

    private HingeJoint[] joints;

    // 1度限りの実行
    public override void Initialize()
    {
        // ジョイントの取得
        joints = GetComponentsInChildren<HingeJoint>(); // クラス<型>
    }

    // 1エピソード限りの実行
    public override void OnEpisodeBegin()
    {
        JointMotor motor;

        foreach (HingeJoint joint in joints)
        {
            motor = joint.motor;
            motor.force = 0;
            motor.targetVelocity = 0;
            joint.motor = motor;
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

        JointMotor motor;

        for (int i = 0; i < actionBuffers.ContinuousActions.Length; i++)
        {
            motor = joints[i].motor;
            motor.force = actionBuffers.ContinuousActions[i] * setForce;
            motor.targetVelocity = 0;
            joints[i].motor = motor;
        }
    }
}
