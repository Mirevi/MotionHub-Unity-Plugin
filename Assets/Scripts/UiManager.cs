using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{

    public Canvas uiCanvas;
    private bool isCanvasEnabled = false;

    private void Start()
    {

        uiCanvas.enabled = false;

    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Backslash))
        {

            isCanvasEnabled = !isCanvasEnabled;

            uiCanvas.enabled = isCanvasEnabled;

        }        
    }
}
