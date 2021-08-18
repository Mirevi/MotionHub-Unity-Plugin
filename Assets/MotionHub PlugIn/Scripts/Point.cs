using System;
using UnityEngine;

namespace MMH {
    public class Point : MonoBehaviour {

        public int pointID;

        public int type;
        public PointType pointType;

        public Avatar.Joint.JointName jointName;

        public int customInt;
        public float customFloat;

        new string name;

        float timeLastUpdated = 0.0f;

        public bool Active;

        [SerializeField]
        GameObject debugMeshAxis;

        [SerializeField]
        Material debugMaterial;

        GameObject debugMesh;

        GameObject debugSphere;

        public enum PointType {
            Undefined,
            HMD,
            Controller,
            Tracker,
            Camera,
            Rigidbody,
            Marker
        };


        void Awake() {
            if (debugMeshAxis == null) {
                debugMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);

                debugMesh.transform.parent = transform;
                debugMesh.transform.localScale = Vector3.one * 0.075f;
            } else {
                debugMesh = Instantiate(debugMeshAxis, transform);

            }

            debugMesh.transform.localPosition = Vector3.zero;
            if (debugMaterial != null) {
                debugMesh.GetComponent<MeshRenderer>().material = debugMaterial;
            }

            debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugSphere.transform.parent = transform;
            debugSphere.transform.localScale = Vector3.one * 0.075f;
            debugSphere.transform.localPosition = Vector3.zero;

            if (debugMaterial != null) {
                debugSphere.GetComponent<MeshRenderer>().material = debugMaterial;
            }
            debugSphere.SetActive(false);
        }

        public void addTimeLastUpdated() {
            timeLastUpdated += Time.deltaTime;
        }

        public float getTimeLastUpdated() {
            return timeLastUpdated;
        }

        public void setName(string name) {
            this.name = name;
            gameObject.name = name;
        }

        public void setPose(Vector3 position, Quaternion rotation) {
            transform.localPosition = position;
            transform.localRotation = rotation;

            timeLastUpdated = 0.0f;
        }

        public void setType(int type) {
            this.type = type;
            pointType = (PointType)type;

            switch (pointType) {
                case PointType.Marker:
                    debugMesh.SetActive(false);
                    debugSphere.SetActive(true);
                    break;
                default:
                    debugMesh.SetActive(true);
                    debugSphere.SetActive(false);
                    break;
            }
        }

        public void setValid(int valid) {
            Active = (valid == 1 ? true : false);

            if (valid == 1) {
                setMeshActive(true);
            } else {
                setMeshActive(false);
            }
        }

        void setMeshActive(bool active) {
            if(!active) {
                debugMesh.SetActive(false);
                debugSphere.SetActive(false);

                return;
            }

            switch (pointType) {
                case PointType.Marker:
                    debugMesh.SetActive(false);
                    debugSphere.SetActive(true);
                    break;
                default:
                    debugMesh.SetActive(true);
                    debugSphere.SetActive(false);
                    break;
            }
        }

        public void setCustomInt(int customInt) {
            this.customInt = customInt;
            jointName = (Avatar.Joint.JointName)customInt;
        }

        public void setCustomFloat(float customFloat) {
            this.customFloat = customFloat;
        }
    }
}