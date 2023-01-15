using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        float dx = Input.GetAxis("Horizontal") * Time.deltaTime * 3;
        float dz = Input.GetAxis("Vertical") * Time.deltaTime * 3;
        transform.position = new Vector3 (
        transform.position.x + dx, 4, transform.position.z + dz
        );
    }
}
