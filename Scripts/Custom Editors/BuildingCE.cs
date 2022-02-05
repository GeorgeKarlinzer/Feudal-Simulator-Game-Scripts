#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace CustomEditors
{
    [CustomEditor(typeof(Building))]
    [CanEditMultipleObjects]
    public class BuildingCE : Editor
    {
        private Building building;
        private SerializedObject buildingSO;
        private SerializedProperty size;
        private void OnEnable()
        {
            Enable();
        }

        protected virtual void Enable()
        {
            building = (Building)target;
            buildingSO = new SerializedObject(building);
            size = buildingSO.FindProperty("size");
        }

        string buttonText = "Show";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Цены", GUILayout.Width(50));

                if (GUILayout.Button(buttonText, GUILayout.Height(20), GUILayout.Width(80)))
                {
                    buttonText = buttonText == "Show" ? "Hide" : "Show";
                }
            }
            EditorGUILayout.EndHorizontal();

            if (buttonText == "Hide")
            {
                EditorGUILayout.BeginVertical("TextArea");

                if (building.prices.Count == 0)
                    EditorGUILayout.LabelField("Список цен пуст");
                else
                {
                    for (int i = 0; i < building.prices.Count; i++)
                        DrawPrice(i);
                }

                EditorGUILayout.EndVertical();

                DrawNewPrice();
            }

            size.vector2IntValue = EditorGUILayout.Vector2IntField("Размер комнаты", size.vector2IntValue);
            buildingSO.ApplyModifiedProperties();

            building.MaxHealth = EditorGUILayout.IntField("Здоровье", building.MaxHealth, GUILayout.Width(200));

            GUILayout.Label("");
            GUILayout.Label("Описание комнаты");
            building.info.description = GUILayout.TextArea(building.info.description, GUILayout.MinHeight(100));
            building.info.panel = (GameObject)EditorGUILayout.ObjectField("Панель информации", building.info.panel, typeof(GameObject), false);
            building.info.classSprite = (Sprite)EditorGUILayout.ObjectField("Спрайт класса", building.info.classSprite, typeof(Sprite), false);

            if (GUI.changed)
                SetObjDirty();
        }

        int selected = 0;
        int resAmount = 0;
        private void DrawNewPrice()
        {
            GUI.backgroundColor = new Color(0.95f, 0.95f, 0.95f);
            EditorGUILayout.BeginVertical("Button");
            GUI.backgroundColor = Color.white;

            EditorGUILayout.LabelField("Создать", new GUIStyle("boldLabel"));

            string[] options = new string[]
            {
                "wood", "stone", "iron", "food", "mood", "water", "gold", "human", "sword", "bow", "pike"
            };

            selected = EditorGUILayout.Popup("Ресурс", selected, options, GUILayout.Width(300));
            resAmount = EditorGUILayout.IntField("Количество", resAmount, GUILayout.Width(300));
            if (GUILayout.Button("Создать"))
            {
                foreach (Price price in building.prices)
                    if (price.name == options[selected])
                    {
                        Debug.LogError("Цена с этим ресурсом уже существует!");
                        return;
                    }
                    else
                        if (resAmount <= 0)
                    {
                        Debug.LogError("Некоректное количество!");
                        return;
                    }

                building.prices.Add(new Price(options[selected], resAmount));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawPrice(int index)
        {
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
            GUILayout.BeginVertical("TextArea");
            GUI.backgroundColor = Color.white;
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Ресурс", GUILayout.Width(167));
                    EditorGUILayout.LabelField(building.prices[index].name, new GUIStyle("boldLabel"));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Количество", GUILayout.Width(167));
                    EditorGUILayout.LabelField(building.prices[index].amount.ToString(), new GUIStyle("boldLabel"));
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("X", GUILayout.Width(20)))
                    building.prices.Remove(building.prices[index]);
            }
            GUILayout.EndVertical();
        }

        private void SetObjDirty()
        {
            EditorSceneManager.MarkSceneDirty(building.gameObject.scene);
            EditorUtility.SetDirty(building.gameObject);
        }
    }
}
#endif