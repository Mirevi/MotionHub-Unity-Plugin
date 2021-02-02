using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderRemoteController : MonoBehaviour
{
    OSC oscmanager;

    OscMessage mess;

    private void Start()
    {
        // find oscmanager refference
        oscmanager = GameObject.Find("$oscmanager").GetComponent<OSC>();

        mess = new OscMessage();

        mess.address = "/mh/Recorder/";
        mess.values.Add("toggleRec");

    }
    public void ToggleRecorder()
    {
        oscmanager.Send(mess);
    }


}
