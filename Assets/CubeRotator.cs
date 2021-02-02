using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour
{

    public GameObject parentCube;
    public GameObject cube;



    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.P))
        {
            parentCube.transform.Rotate(Vector3.up, 1.0f);
        }

        if (Input.GetKey(KeyCode.C))
        {
            cube.transform.Rotate(Vector3.up, 1.0f);
        }
    }
}
