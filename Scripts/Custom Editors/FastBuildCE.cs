#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomEditors
{
    [CustomEditor(typeof(FastBuild))]
    public class FastBuildCE : Editor
    {
        FastBuild origin;
        string[] angles = { "0°", "90°", "180°", "270°" };

        private void OnEnable()
        {
            origin = (FastBuild)target;
            savedPatters = Directory.GetFiles("Assets/FastBuild/", "*.json");
            for (int i = 0; i < savedPatters.Length; i++)
                savedPatters[i] = Path.GetFileNameWithoutExtension(savedPatters[i]);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Комнаты для быстрого строительства");

            if (GUILayout.Button("Сортировать"))
                Sort();

            for (int i = 0; i < origin.queue.Count; i++)
            {
                FastBuild.FastBuildBuilding fast = origin.queue[i];

                GUI.backgroundColor = new Color(1, 1, 1, 0.6f);
                EditorGUILayout.BeginVertical("button");
                GUI.backgroundColor = new Color(1, 1, 1);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Комната", GUILayout.Width(Screen.width / 3f));

                if (GUILayout.Button(fast.building == null ? "..." : fast.building.name, GUILayout.Width(Screen.width / 3f)))
                    MyWindow.ShowWindow(origin, i);

                GUILayout.Label("", GUILayout.Width(Screen.width / 3 - 80));
                GUI.backgroundColor = new Color(1, 0, 0, 0.4f);

                if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    origin.queue.RemoveAt(i--);
                    continue;
                }

                GUI.backgroundColor = new Color(1, 1, 1);
                GUILayout.EndHorizontal();

                fast.pos = EditorGUILayout.Vector2Field("Положение", fast.pos, GUILayout.Width(Screen.width * 3f / 4));
                fast.rotation = new Vector3(0, 0, 90 * EditorGUILayout.Popup("Поворот", (int)(fast.rotation.z / 90), angles, GUILayout.Width(Screen.width * 2f / 3)));

                if (fast.building != null && fast.building.GetComponent<IEmployable>() != null)
                    fast.employedCount = EditorGUILayout.IntSlider("Количество рабочих", fast.employedCount, 0, fast.building.GetComponent<IEmployable>().MaxWorkers, GUILayout.Width(Screen.width * 2f / 3));
                else
                    GUILayout.Label("");

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Добавить"))
                origin.queue.Add(new FastBuild.FastBuildBuilding());

            GUILayout.EndVertical();

            GUILayout.Label("");

            saveName = EditorGUILayout.TextField("Название сохранения", saveName);
            if (GUILayout.Button("Сохранить"))
                if (saveName != "")
                    SavePattern();
                else
                    Debug.LogError($"Uncorrect filename!");

            index = EditorGUILayout.Popup("Название сохранения", index, savedPatters);
            if (GUILayout.Button("Загрузить"))
                LoadPattern();

            GUILayout.Label("");
            GUI.backgroundColor = new Color(1, 0, 0, 0.6f);

            if (GUILayout.Button("Очистить"))
                origin.queue = new List<FastBuild.FastBuildBuilding>(0);
        }
        int index = 0;
        string[] savedPatters;
        string saveName = "";

        private void LoadPattern()
        {
            origin.queue = new List<FastBuild.FastBuildBuilding>(0);
            SavedData data = Load.LoadFileJSON<SavedData>(Application.dataPath + "/FastBuild/" + savedPatters[index] + ".json");
            for (int i = 0; i < data.employedCount.Count; i++)
            {
                FastBuild.FastBuildBuilding fast = new FastBuild.FastBuildBuilding();
                fast.employedCount = data.employedCount[i];
                fast.rotation = data.rotation[i];
                fast.pos = data.pos[i];
                string[] search_results = System.IO.Directory.GetFiles("Assets/Prefabs/", "*.prefab", System.IO.SearchOption.AllDirectories);
                foreach (var s in search_results)
                    if(s.Contains(data.prefabName[i]))
                        fast.building = (GameObject)AssetDatabase.LoadAssetAtPath(s, typeof(GameObject));

                origin.queue.Add(fast);
            }
        }

        private void SavePattern()
        {
            Save.SaveFileJSON(new SavedData(origin.queue), Application.dataPath + "/FastBuild/" + saveName + ".json");
            saveName = "";
            savedPatters = Directory.GetFiles("Assets/FastBuild/", "*.json");
            for (int i = 0; i < savedPatters.Length; i++)
                savedPatters[i] = Path.GetFileNameWithoutExtension(savedPatters[i]);
        }

        private void Sort()
        {
            int count = origin.queue.Count;
            for (int i = 0; i < count; i++)
            {
                if (origin.queue[i].building == null)
                {
                    origin.queue.RemoveAt(i--);
                    count--;
                }

                if (origin.queue[i].building.GetComponent<Barrack>())
                {
                    var item = origin.queue[i];
                    origin.queue.RemoveAt(i);
                    origin.queue.Insert(0, item);
                }
                if (origin.queue[i].building.GetComponent<Door>())
                {
                    var item = origin.queue[i];
                    origin.queue.RemoveAt(i);
                    i--;
                    count--;
                    origin.queue.Insert(origin.queue.Count, item);
                }
            }
        }
    }

    public class SavedData
    {
        public SavedData() { }
        public SavedData(List<FastBuild.FastBuildBuilding> queue)
        {
            foreach (var q in queue)
            {
                prefabName.Add(q.building.name);
                pos.Add(q.pos);
                rotation.Add(q.rotation);
                employedCount.Add(q.employedCount);
            }
        }
        public List<string> prefabName = new List<string>();
        public List<Vector3> pos = new List<Vector3>();
        public List<Vector3> rotation = new List<Vector3>();
        public List<int> employedCount = new List<int>();
    }

    public class MyWindow : EditorWindow
    {
        private static List<GameObject> prefabs = new List<GameObject>(0);
        private static FastBuild fastBuild;
        private static int index;

        [MenuItem("Window/My Window")]
        public static void ShowWindow(FastBuild fast, int i)
        {
            fastBuild = fast;
            index = i;

            GetWindow(typeof(MyWindow));
            if (prefabs.Count == 0)
                GetPrefabs();
        }

        private static void GetPrefabs()
        {

            string[] search_results = System.IO.Directory.GetFiles("Assets/Prefabs/", "*.prefab", System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < search_results.Length; i++)
            {
                GameObject prefab;
                prefab = (GameObject)AssetDatabase.LoadAssetAtPath(search_results[i], typeof(GameObject));
                if (prefab.GetComponent<Building>() || prefab.GetComponent<Door>())
                    prefabs.Add(prefab);
            }
        }

        void OnGUI()
        {
            minSize = new Vector2(500, prefabs.Count * 20 + 22);
            maxSize = new Vector2(500, prefabs.Count * 20 + 22);
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (GUILayout.Button(prefabs[i].name))
                {
                    fastBuild.queue[index].building = prefabs[i];
                    Close();
                }
            }
        }
    }
}
#endif