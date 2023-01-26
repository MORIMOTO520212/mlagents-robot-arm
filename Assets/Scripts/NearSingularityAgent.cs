using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class NearSingularityAgent : Agent
{
    public GameObject j1;
    public GameObject j2;
    public GameObject j3;
    public GameObject j4;
    public GameObject j5;
    public GameObject endEffector;
    public GameObject goal1;
    public GameObject goal2;

    private Rigidbody axisRb1;
    private Rigidbody axisRb2;
    private Rigidbody axisRb3;
    private Rigidbody axisRb4;
    private Rigidbody axisRb5;

    private Vector3 basePosition;

    private float prevBest;

    public override void Initialize()
    {
        axisRb1 = j1.GetComponent<Rigidbody>();
        axisRb2 = j2.GetComponent<Rigidbody>();
        axisRb3 = j3.GetComponent<Rigidbody>();
        axisRb4 = j4.GetComponent<Rigidbody>();
        axisRb5 = j5.GetComponent<Rigidbody>();
        basePosition = this.gameObject.GetComponent<Transform>().position;
    }

    public override void OnEpisodeBegin()
    {
        // 姿勢の初期化
        j1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        j2.transform.rotation = Quaternion.Euler(0f, -20f, 0f);
        j3.transform.rotation = Quaternion.Euler(0f, -50f, 0f);
        j4.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        j5.transform.rotation = Quaternion.Euler(0f, 0f, 7f);

        // 力 リセット
        axisRb1.velocity = Vector3.zero;
        axisRb2.velocity = Vector3.zero;
        axisRb3.velocity = Vector3.zero;
        axisRb4.velocity = Vector3.zero;
        axisRb5.velocity = Vector3.zero;
        axisRb1.angularVelocity = Vector3.zero;
        axisRb2.angularVelocity = Vector3.zero;
        axisRb3.angularVelocity = Vector3.zero;
        axisRb4.angularVelocity = Vector3.zero;
        axisRb5.angularVelocity = Vector3.zero;

        // 前の距離prevBestの初期化
        prevBest = Vector3.Distance(endEffector.transform.position, goal1.transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(j1.transform.position - basePosition);
        sensor.AddObservation(j2.transform.position - basePosition);
        sensor.AddObservation(j3.transform.position - basePosition);
        sensor.AddObservation(j4.transform.position - basePosition);
        sensor.AddObservation(j5.transform.position - basePosition);
        sensor.AddObservation(j1.transform.rotation);
        sensor.AddObservation(j2.transform.rotation);
        sensor.AddObservation(j3.transform.rotation);
        sensor.AddObservation(j4.transform.rotation);
        sensor.AddObservation(j5.transform.rotation);
        sensor.AddObservation(axisRb1.velocity);
        sensor.AddObservation(axisRb2.velocity);
        sensor.AddObservation(axisRb3.velocity);
        sensor.AddObservation(axisRb4.velocity);
        sensor.AddObservation(axisRb5.velocity);
        sensor.AddObservation(goal1.transform.position - basePosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float beginDistance = Vector3.Distance(endEffector.transform.position, goal1.transform.position);

        // 値を-1f~1fに正規化
        float force = 150f;
        var torque = Mathf.Clamp( actions.ContinuousActions[0], -1f, 1f ) * force;
        axisRb1.AddTorque(new Vector3(0f, torque, 0f));

        torque = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * force;
        axisRb2.AddTorque(new Vector3(0f, 0f, torque));

        torque = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * force;
        axisRb3.AddTorque(new Vector3(0f, 0f, torque));

        torque = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f) * force;
        axisRb4.AddTorque(new Vector3(0f, torque, 0f));

        torque = Mathf.Clamp(actions.ContinuousActions[4], -1f, 1f) * force;
        axisRb5.AddTorque(new Vector3(0f, 0f, torque));


        // ターゲットに接触したらプラス報酬
        float distance = Vector3.Distance(endEffector.transform.position, goal1.transform.position);

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
        if (distance < 0.1f)
        {
            AddReward(0.6f);
            Debug.Log("Goal");
        }

        // 床面に当たった場合、ペナルティ
        if (endEffector.transform.position.y < 0.3)
        {
            AddReward(-0.1f);
            EndEpisode();
        }

        AddReward(-0.0001f);
    }

    public void Update()
    {
    }
}
