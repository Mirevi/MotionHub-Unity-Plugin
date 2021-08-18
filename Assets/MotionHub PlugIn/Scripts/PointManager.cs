using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMH {
    public class PointManager : MonoBehaviour {

        #region Serialize Fields
        [SerializeField]
        Point pointPrefab;

        [Range(0.01f, 10.0f)]
        public float timeout = 0.25f;

        [SerializeField]
        bool enableDebug = true;
        #endregion

        #region Private Fields
        OSC oscmanager;

        Dictionary<int, Point> pointPool;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake() {
            pointPool = new Dictionary<int, Point>();
        }

        void Start() {
            // find oscmanager refference
            oscmanager = GameObject.Find("$oscmanager").GetComponent<OSC>();

            // set callback function for on receive osc messages
            oscmanager.SetAddressHandler("/mh/point/", OnReceivePoint);
        }

        void Update() {
            foreach (KeyValuePair<int, Point> currPoint in pointPool) {

                Point currPointClass = currPoint.Value;

                if (currPointClass.getTimeLastUpdated() >= timeout) {
                    pointPool.Remove(currPoint.Key);
                    Destroy(currPointClass.gameObject);

                    if (enableDebug)
                        Debug.Log("[INFO]: PointManager::Update(): Removed point with id = " + currPoint.Key);

                    return;
                } else {
                    currPointClass.addTimeLastUpdated();
                }
            }
        }
        #endregion

        void OnReceivePoint(OscMessage message) {

            // get sent point count
            int pointCount = message.GetInt(0);

            for (int i = 0; i < pointCount; i++) {

                // get point id
                int id = message.GetInt(i * 12 + 8);

                // get global position
                Vector3 position = new Vector3(
                        -message.GetFloat(i * 12 + 1),
                        message.GetFloat(i * 12 + 2),
                        message.GetFloat(i * 12 + 3)
                );

                // get global rotation
                Quaternion rotation = new Quaternion(
                        message.GetFloat(i * 12 + 6),   // z
                        message.GetFloat(i * 12 + 7),   // w
                        message.GetFloat(i * 12 + 4),   // x
                        message.GetFloat(i * 12 + 5)    // y
                );

                // get valid flag
                int valid = message.GetInt(i * 12 + 9);

                // check if point is already present
                Point currPoint;
                pointPool.TryGetValue(id, out currPoint);

                // update point if present
                if (currPoint != null) {

                    // update position & rotation
                    currPoint.SetPose(position, rotation);

                    currPoint.SetValid(valid);
                }
                // pont does not exist: create new
                else {
                    // instantiate new avatar and add it to point pool
                    currPoint = Instantiate(pointPrefab, transform);
                    pointPool.Add(id, currPoint);

                    // update position & rotation
                    currPoint.SetPose(position, rotation);

                    // set point object name
                    currPoint.SetName("point_" + id);

                    currPoint.pointID = id;

                    // set ccustom flags
                    currPoint.SetType(message.GetInt(i * 12 + 10));
                    currPoint.SetCustomInt(message.GetInt(i * 12 + 11));
                    currPoint.SetCustomFloat(message.GetFloat(i * 12 + 12));

                    currPoint.SetValid(valid);

                    if (enableDebug)
                        Debug.Log("[INFO]: PointManager::OnReceivePoint(): Created new point with id = " + id);
                }
            }
        }
    }
}