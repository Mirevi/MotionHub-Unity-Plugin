using System;
using UnityEngine;

namespace MMH {
    public class Point : Streamable {

        public int pointID;

        public int type;
        public PointType pointType;

        public Avatar.Joint.JointName jointName;

        public int customInt;
        public float customFloat;

        public bool Active;

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

        #region MonoBehaviour Callbacks
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
        #endregion

        public void SetPose(Vector3 position, Quaternion rotation) {
            transform.localPosition = position;
            transform.localRotation = rotation;

            resetTimeLastUpdated();
        }

        public void SetType(int type) {
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

        public bool GetValid() {
            return Active;
        }

        public void SetValid(int valid) {
            Active = (valid == 1 ? true : false);

            if (valid == 1) {
                SetMeshActive(true);
            } else {
                SetMeshActive(false);
            }
        }

        public int GetCustomInt() {
            return customInt;
        }

        public void SetCustomInt(int customInt) {
            this.customInt = customInt;
            jointName = (Avatar.Joint.JointName)customInt;
        }

        public float GetCustomFloat() {
            return customFloat;
        }

        public void SetCustomFloat(float customFloat) {
            this.customFloat = customFloat;
        }

        void SetMeshActive(bool active) {
            if (!active) {
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
    }
}