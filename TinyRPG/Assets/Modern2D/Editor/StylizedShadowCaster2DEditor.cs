using UnityEditor;
using UnityEngine;

namespace Modern2D
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(StylizedShadowCaster2D))]
    public class StylizedShadowCaster2DEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("CreateShadow"))
                foreach (StylizedShadowCaster2D caster in targets)
                    caster.CreateShadow();
            if (GUILayout.Button("RebuildShadow"))
                foreach (StylizedShadowCaster2D caster in targets)
                    caster.RebuildShadow();

            StylizedShadowCaster2D system = (StylizedShadowCaster2D)target;
            GUILayout.Space(10);
            system.extendedProperties = GUILayout.Toggle(system.extendedProperties, "Per Every Shadow Properties [Warning : GPU Expensive] \n (if possible, keep shadows the same and change values via StylizedLightingSystem2D)");

            if (system.extendedProperties) 
            {
                system.SetCallbacks();

                GUILayout.Space(5); system._shadowColor.value = EditorGUILayout.ColorField("Ambient Color", system._shadowColor.value);
                GUILayout.Space(5); system._shadowReflectiveness.value = EditorGUILayout.Slider("Shadow Reflectiveness", system._shadowReflectiveness.value, 0, 1);
                GUILayout.Space(5); system._shadowAlpha.value = EditorGUILayout.Slider("Shadow Alpha", system._shadowAlpha.value, 0, 1);
                GUILayout.Space(5); system._shadowNarrowing.value = EditorGUILayout.Slider("Shadow Narrowing", system._shadowNarrowing.value, 0, 1);
                GUILayout.Space(5); system._shadowFalloff.value = EditorGUILayout.Slider("Shadow Falloff", system._shadowFalloff.value, 0, 15);
            }
        }
    }

}