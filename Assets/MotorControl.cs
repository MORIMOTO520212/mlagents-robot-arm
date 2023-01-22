using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorControl : MonoBehaviour
{
    public int motor(GameObject axis, float target_angle, int rotateSpeed = 10)
    {
        float current_angle = axis.transform.localEulerAngles.z;

        // 回転開始
        if (current_angle > target_angle)
            axis.transform.Rotate(0, 0, rotateSpeed * -1);

        else if (current_angle < target_angle)
            axis.transform.Rotate(0, 0, rotateSpeed * 1);

        else return 1;

        // 待機
        while ((int)current_angle == (int)target_angle)
            current_angle = axis.transform.localEulerAngles.z;

        // 回転停止
        axis.transform.Rotate(0, 0, 0);

        return 1;
    }
}
