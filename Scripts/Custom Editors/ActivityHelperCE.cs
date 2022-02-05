
#if TEST
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(ActivityHelper))]
public class ActivityHelperCE : Editor
{
    Building origin;
    List<Activity> activities = new List<Activity>();

    private void OnEnable()
    {

        origin = ((ActivityHelper)target).GetComponent<Building>();

        origin.graphics.Clear();

        if (origin.GetComponent<SpriteRenderer>())
            origin.graphics.Add(origin.GetComponent<SpriteRenderer>());

        AddChild(origin.transform);

        if (GUI.changed)
            SetObjDirty();

        DestroyImmediate(origin.GetComponent<ActivityHelper>(), true);

        //activities = origin.activities;
    }

    public void AddChild(Transform transform)
    {
        foreach (Transform t in transform)
        {
            if (!t.GetComponent<DoorPlace>())
            {
                if (t.childCount > 0)
                    AddChild(t);

                if (t.GetComponent<SpriteRenderer>())
                    origin.graphics.Add(t.GetComponent<SpriteRenderer>());
            }
        }
    }

    public override void OnInspectorGUI()
    {
        //GUILayout.Label("Количество ресурсов:");
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("В час:", GUILayout.Width(100));
        //origin.amountPerDay = (EditorGUILayout.FloatField(origin.amountPerDay / 24f, GUILayout.MinWidth(10), GUILayout.MaxWidth(60)) * 24);
        //GUILayout.Label("В день:", GUILayout.Width(100));
        //origin.amountPerDay = EditorGUILayout.FloatField(origin.amountPerDay, GUILayout.MinWidth(10), GUILayout.MaxWidth(60));

        //GUILayout.EndHorizontal();

        //for (int i = 0; i < activities.Count; i++)
        //{
        //    Activity activity = activities[i];

        //    GUILayout.BeginVertical("HelpBox");
        //    GUILayout.Label("Название действия (для себя)");
        //    activity.activityName = EditorGUILayout.TextField(activity.activityName);
        //    GUILayout.Label("Место работы");
        //    activity.place = EditorGUILayout.ObjectField(activity.place, typeof(Transform), true) as Transform;
        //    GUILayout.Label("Название триггера анимации");
        //    activity.mainActionTrigger = EditorGUILayout.TextField(activity.mainActionTrigger);
        //    GUILayout.Label("Время выполнения анимации");
        //    activity.time = EditorGUILayout.FloatField(activity.time);


        //    GUILayout.Space(10);
        //    if (GUILayout.Button("Удалить", GUILayout.Width(70)))
        //        activities.RemoveAt(i--);

        //    GUILayout.EndVertical();
        //}

        //if (GUILayout.Button("Добавить действие"))
        //    origin.activities.Add(new Activity());

        //serializedObject.ApplyModifiedProperties();
        //if (GUI.changed)
        //    SetObjDirty();
    }

    private void SetObjDirty()
    {
        EditorSceneManager.MarkSceneDirty(origin.gameObject.scene);
        EditorUtility.SetDirty(origin.gameObject);
    }
}
#endif
#endif