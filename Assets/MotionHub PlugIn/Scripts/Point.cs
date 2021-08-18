using System;
using UnityEngine;

namespace MMH {
    public class Point : Streamable {

        public int pointID;

        public int type;
        public PointType pointType;

        public Avatar.Joint.JointName jointName;

        public bool Active { get; private set; }

        #region Private Fields
        int customInt;

        float customFloat;

        GameObject debugMesh;

        GameObject debugSphere;
        #endregion

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
            // Init Mesh if configured
            if (debugMeshAxis != null) {
                debugMesh = Instantiate(debugMeshAxis, transform);
            }
            // Create Primitive if not
            else { 
                debugMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);

                debugMesh.transform.parent = transform;
                debugMesh.transform.localScale = Vector3.one * 0.075f;
            }

            debugMesh.transform.localPosition = Vector3.zero;
            if (debugMaterial != null) {
                debugMesh.GetComponent<MeshRenderer>().material = debugMaterial;
            }

            // Create & Init Debug Sphere GameObject
            debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugSphere.transform.parent = transform;
            debugSphere.transform.localScale = Vector3.one * 0.075f;
            debugSphere.transform.localPosition = Vector3.zero;

            // Assign Deug Material if configured
            if (debugMaterial != null) {
                debugSphere.GetComponent<MeshRenderer>().material = debugMaterial;
            }

            // Disable Debug Sphere
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