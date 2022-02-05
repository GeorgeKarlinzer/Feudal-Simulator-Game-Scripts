#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomEditors
{
    [CustomEditor(typeof(Barrack))]
    [CanEditMultipleObjects]
    public class BarrackCE : BuildingCE
    {
        private Barrack barrack;

        SerializedObject barrackSO;
        SerializedProperty maxAmount;
        SerializedProperty humanPrefab;
        SerializedProperty destination;
        SerializedProperty workerController;
        protected override void Enable()
        {
            base.Enable();
            barrack = (Barrack)target;
            barrackSO = new SerializedObject(barrack);
            maxAmount = barrackSO.FindProperty("maxHumans");
            humanPrefab = barrackSO.FindProperty("humanPrefab");
            destination = barrackSO.FindProperty("destination");
            workerController = barrackSO.FindProperty("workerController");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;
            GUILayout.Label("");

            var content = new GUIContent("Max", "Максимальное количество человек");
            maxAmount.intValue = EditorGUILayout.IntField(content, maxAmount.intValue);

            EditorGUILayout.PropertyField(humanPrefab);
            EditorGUILayout.PropertyField(destination);
            EditorGUILayout.PropertyField(workerController);

            barrackSO.ApplyModifiedProperties();

            barrack.humanPrice.amount = EditorGUILayout.IntField("Цена человека", barrack.humanPrice.amount);

            if (GUI.changed)
                SetObjDirty();
        }
        private void SetObjDirty()
        {
            EditorSceneManager.MarkSceneDirty(barrack.gameObject.scene);
            EditorUtility.SetDirty(barrack.gameObject);
        }
    }
}
#endif