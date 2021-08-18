using System.Collections;
using UnityEngine;

namespace MMH {
    public class Streamable : MonoBehaviour {

        #region Serialize Fields
        [SerializeField]
        protected GameObject debugMeshAxis;

        [SerializeField]
        protected Material debugMaterial;
        #endregion

        #region Private Fields
        private new string name;

        private float timeLastUpdated = 0.0f;
        #endregion

        #region Public Methods
        public string GetName() {

            return name;
        }

        public void SetName(string name) {

            this.name = name;
            gameObject.name = name;
        }

        public void addTimeLastUpdated() {

            timeLastUpdated += Time.deltaTime;
        }

        public void resetTimeLastUpdated() {

            timeLastUpdated = 0.0f;
        }

        public float getTimeLastUpdated() {

            return timeLastUpdated;
        }
        #endregion
    }
}