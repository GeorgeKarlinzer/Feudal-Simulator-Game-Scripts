#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomEditors
{
    [CustomEditor(typeof(ResManager))]
    [CanEditMultipleObjects]
    public class ResourcesManagerCE : Editor
    {
        private ResManager origin;

        private void OnEnable()
        {
            origin = (ResManager)target;
            foreach (string key in ResManager.res.Keys)
            {
                flags.Add(key, false);
            }
        }
        Dictionary<string, bool> flags = new Dictionary<string, bool>();
        bool mainFlag;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Debug"))
                mainFlag = !mainFlag;

            if (mainFlag)
                foreach (string key in ResManager.res.Keys)
                {
                    GUILayout.BeginVertical("box");

                    if (GUILayout.Button($"{ResManager.res[key].Name} ({ResManager.res[key].GetStorages().Count})"))
                        flags[key] = !flags[key];

                    if (flags[key])
                        foreach (var s in ResManager.res[key].GetStorages())
                            if (s.Destination != null)
                            {
                                GUILayout.Label("Name\t" + s.Destination.parent.name);
                                GUILayout.Label("Amount\t" + s.GetAmount(key));
                            }

                    GUILayout.EndVertical();
                }
        }
    }
}
#endif