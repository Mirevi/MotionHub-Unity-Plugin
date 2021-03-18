using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMH
{
    public class RecorderRemoteController : MonoBehaviour
    {
        OSC oscmanager;

        OscMessage mess;

        private void Start()
        {
            // find oscmanager refference
            oscmanager = GameObject.Find("$oscmanager").GetComponent<OSC>();

            //create OSC message
            mess = new OscMessage();

            //fill message header and content
            mess.address = "/mh/Recorder/";
            mess.values.Add("toggleRec");

        }
        public void ToggleRecorder()
        {
            //send message
            oscmanager.Send(mess);
        }


    }
}