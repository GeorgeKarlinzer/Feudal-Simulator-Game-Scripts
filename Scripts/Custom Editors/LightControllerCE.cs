#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomEditors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightController))]
    public class LightControllerCE : Editor
    {
        private void OnEnable()
        {
            var origin = (LightController)target;
            if (origin.image == null)
            {
                origin.image = origin.GetComponent<SpriteRenderer>();
                if (origin.GetComponent<Building>())
                    origin.daySprite = origin.image.sprite;
            }
        }
    }
}
#endif