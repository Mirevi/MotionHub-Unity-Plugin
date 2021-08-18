using UnityEngine;
using UnityEditor;

namespace MMH {
    [CustomEditor(typeof(PointManager))]
    public class PointManagerEditor : Editor {

        PointManager pointManager;

        void OnEnable() {
            pointManager = ((PointManager)target);
        }

        public override void OnInspectorGUI() {
            // Update targetObject & draw default inspector exept pointTypeMask field
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "pointTypeMask");

            var selectedPointTypeFlags = pointManager.GetPointTypeMask();

            // Point Type LayerMask Field
            var newPointTypeFlags = PointTypeMaskField("Enabled Types", selectedPointTypeFlags);

            // Apply modified properties if changed
            if (!newPointTypeFlags.Equals(selectedPointTypeFlags)) {
                pointManager.UpdatePointTypeMask(newPointTypeFlags);
                serializedObject.ApplyModifiedProperties();
            }
        }

        LayerMask PointTypeMaskField(string label, LayerMask selected) {

            var pointTypes = System.Enum.GetNames(typeof(Point.PointType));

            selected.value = EditorGUILayout.MaskField(label, selected.value, pointTypes);

            return selected;
        }
    }
}