using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Modern2D
{

    [CustomEditor(typeof(LightingSystem))]
    public class LightingSystemEditor : Editor
    {

        [SerializeField] Texture2D myTexture;
        public override void OnInspectorGUI()
        {

            GUIStyle style = new GUIStyle();
            style.stretchWidth = true;
            style.stretchHeight = true;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(myTexture, style);
            GUILayout.Space(10);

            base.OnInspectorGUI();

            GUIStyle header = new GUIStyle();
            header.fontStyle = FontStyle.Bold;
            header.normal.textColor = new Color(150 / 255f, 125 / 255f, 255 / 255f);
            GUILayout.Label("OPTIONS", header, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

            LightingSystem system = (LightingSystem)target;

            GUILayout.Space(5); system._shadowColor.value = EditorGUILayout.ColorField("Ambient Color", system._shadowColor.value);
            GUILayout.Space(5); system._shadowReflectiveness.value = EditorGUILayout.Slider("Shadow Reflectiveness", system._shadowReflectiveness.value, 0, 1);
            GUILayout.Space(5); system._shadowAlpha.value = EditorGUILayout.Slider("Shadow Alpha", system._shadowAlpha.value, 0, 1);
            GUILayout.Space(5); system._shadowLength.value = EditorGUILayout.Slider("Shadow Length", system._shadowLength.value, 0, 5);
            GUILayout.Space(5); system._shadowNarrowing.value = EditorGUILayout.Slider("Shadow Narrowing", system._shadowNarrowing.value, 0, 1);
            GUILayout.Space(5); system._shadowFalloff.value = EditorGUILayout.Slider("Shadow Falloff", system._shadowFalloff.value, 0, 15);
            GUILayout.Space(5); system._shadowAngle.value = EditorGUILayout.Slider("Shadow Angle", system._shadowAngle.value, 0, 90);

            GUILayout.Space(10);
            system.followPlayer = (Transform)EditorGUILayout.ObjectField("Camera Transform", system.followPlayer, typeof(Transform), true);

            GUILayout.Space(15);
            GUILayout.Label("LIGHT SOURCE");

            system.SetCallbacks();
            system.Singleton();


            if (system.isLightDirectional.value = GUILayout.Toggle(system.isLightDirectional.value, " Is Light Directional "))
                DirectionalFields(system);
            else
                SourceFields(system);

            GUILayout.Space(15);

            if (GUILayout.Button("DELETE ALL SHADOWS"))
                foreach (var s in GameObject.FindGameObjectsWithTag("Shadow"))
                    DestroyImmediate(s);

            //this is an example of horribly unreadable code
            //don't try this at home kids
            if (GUILayout.Button("CREATE ALL SHADOWS"))
            {
                foreach (var s in Transform.FindObjectsOfType<StylizedShadowCaster2D>())
                {
                    s.RebuildShadow();
                    system.AddShadow(s.shadowData);
                    system.extendedUpdateThisFrame = true;
                    system.OnShadowSettingsChanged();
                    system.UpdateShadows(Transform.FindObjectsOfType<StylizedShadowCaster2D>().ToDictionary(t => t.transform, t => t.shadowData.shadow));
                }
            }
            if (GUILayout.Button("UPDATE ALL SHADOWS"))
            {
                system.extendedUpdateThisFrame = true;
                system.OnShadowSettingsChanged();
                system.UpdateShadows(Transform.FindObjectsOfType<StylizedShadowCaster2D>().ToDictionary(t => t.transform, t => t.shadowData.shadow));

            }

            SetLayers();
        }

        private void DirectionalFields(LightingSystem system)
        {
            GUILayout.Label("direction:");
            GUILayout.Space(20);
            system.directionalLightAngle.value = GUILayout.HorizontalSlider((system.directionalLightAngle.value), 0, 359);
            GUILayout.Space(20);
        }

        private void SourceFields(LightingSystem system)
        {
            system.source = (Transform)EditorGUILayout.ObjectField("Light Source", system.source, typeof(Transform), true);
            GUILayout.Space(5);
            system.distMinMax.value = EditorGUILayout.Vector2Field("shadow distance min max", system.distMinMax.value);
            GUILayout.Space(5);
            system.shadowLengthMinMax.value = EditorGUILayout.Vector2Field("shadow length multiplier min max", system.shadowLengthMinMax.value);
        }

        private void SetLayers()
        {

            if (!Layers.LayerExists("LightingSystem"))
                if (!Layers.CreateLayer("LightingSystem"))
                    Debug.LogError("Not enough space for the LightingSystem layer, LightingSystem shadow detection won't work propetly \nPlease assign the LightingSystem layer or make space for it in your layers list");

            if (!Layers.LayerExists("2DWater"))
                if (!Layers.CreateLayer("2DWater"))
                    Debug.LogError("Not enough space for the Water layer, water won't work propetly \nPlease assign the Water layer or make space for it in your layers list");

            if (!Layers.TagExists("Shadow"))
                if (!Layers.CreateTag("Shadow"))
                    Debug.LogError("Not enough space for the Shadow tag, System won't be able to find shadows \nPlease assign the shadows tag or make space for it in your tags list");
        }

    }

}