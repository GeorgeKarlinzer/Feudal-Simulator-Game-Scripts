#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomEditors
{
    [CustomEditor(typeof(CameraController))]
    [CanEditMultipleObjects]
    public class CameraControllerCE : Editor
    {
        private CameraController origin;

        private void OnEnable()
        {
            origin = (CameraController)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;
            EditorGUILayout.LabelField("Размеры камеры", new GUIStyle("boldLabel"));

            origin.maxSize = EditorGUILayout.FloatField("Max", origin.maxSize);
            origin.minSize = EditorGUILayout.FloatField("Min", origin.minSize);

            EditorGUILayout.LabelField("");

            EditorGUILayout.LabelField("Скорость камеры", new GUIStyle("boldLabel"));

            origin.speed = EditorGUILayout.FloatField("По плоскости", origin.speed);
            origin.sizeSpeed = EditorGUILayout.FloatField("Размер", origin.sizeSpeed);

            EditorGUILayout.LabelField("");

            origin.borderOffset = EditorGUILayout.FloatField(new GUIContent("Отступ", "Оступ мышки от края экрана для срабатывания перемещения камеры"), origin.borderOffset);

            if (GUI.changed)
                SetObjDirty();
        }

        private void SetObjDirty()
        {
            EditorSceneManager.MarkSceneDirty(origin.gameObject.scene);
            EditorUtility.SetDirty(origin.gameObject);

        }
    }
}
#endif
