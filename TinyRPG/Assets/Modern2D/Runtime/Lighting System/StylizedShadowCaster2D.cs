using UnityEditor;
using UnityEngine;

namespace Modern2D
{

    //  shadow type that is used by stylized lighting 2D system
    //  stylized lighting 2D system needs to be setup beforehand
    //  it also needs an "Shadows" sprite sorting layer and "Shadow" tag
    //  for detailed tutorial please read the whole setup section in my documentation

    public class StylizedShadowCaster2D : MonoBehaviour
    {
        [SerializeField] private ShadowData _shadowData;

        [Tooltip("Color that's applied to shadow color calculation and other shaders")]
        [SerializeField] [HideInInspector] public Cryo<Color> _shadowColor;

        [Tooltip("special abstract property of the shadow that's responsible for the illusion of shadow reflecting shadowcaster")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowReflectiveness;

        [Tooltip("Alpha of shadow color that's applied to shadow color calculation and other shaders")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowAlpha;

        [Tooltip("Shadow Narrowing of the drop shadow in shadowcasters")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowNarrowing;

        [Tooltip("Shadow Falloff of the drop shadow in shadowcasters")]
        [SerializeField] [HideInInspector] public Cryo<float> _shadowFalloff;

        [SerializeField] [HideInInspector] public bool extendedProperties;
        [SerializeField] [HideInInspector] private MaterialPropertyBlock _propBlock;

        public void SetCallbacks()
        {
            _shadowColor.onValueChanged = SetPropBlock;
            _shadowReflectiveness.onValueChanged = SetPropBlock;
            _shadowAlpha.onValueChanged = SetPropBlock;
            _shadowNarrowing.onValueChanged = SetPropBlock;
            _shadowFalloff.onValueChanged = SetPropBlock;
        }

        public void Start()
        {
            if(extendedProperties) SetPropBlock();
        }

        private void OnValidate()
        {
            if (extendedProperties) SetPropBlock();
        }

        public void SetPropBlock() 
        {
            if (!extendedProperties) return;
            _propBlock = new MaterialPropertyBlock();

            if (shadowData.shadow.shadowSr == null || shadowData.shadow.shadowSr.sprite == null) 
            {
                Debug.LogError("Can't change properties : create a shadow first!");
                return;
            }

             shadowData.shadow.shadowSr.GetPropertyBlock(_propBlock);

            _propBlock.SetColor("_shadowBaseColor", _shadowColor.value);
            _propBlock.SetFloat("_shadowBaseAlpha", _shadowAlpha.value);
            _propBlock.SetFloat("_shadowReflectiveness", _shadowReflectiveness.value);
            _propBlock.SetFloat("_shadowNarrowing", _shadowNarrowing.value);
            _propBlock.SetFloat("_shadowFalloff", _shadowFalloff.value);

            shadowData.shadow.shadowSr.SetPropertyBlock(_propBlock);
        }


        /// <summary>
        /// if shadow wasn't created, automatically creates one
        /// </summary>
        public ShadowData shadowData
        {
            get { if (_shadowData == null || _shadowData.shadow.shadowPivot == null) _shadowData = CreateShadowData(); return _shadowData; }
            set { _shadowData = value; }
        }

        /// <summary>
        /// creates shadow or returns existent one
        /// </summary>
        public void CreateShadow()
        {
            LightingSystem.system.AddShadow(shadowData);
        }

        /// <summary>
        /// if shadow exists, it destroys it and creates a new one
        /// </summary>
        public void RebuildShadow()
        {
            if (!Application.isPlaying)
                DestroyImmediate(shadowData.shadow.shadowPivot.gameObject);
            else
                Destroy(shadowData.shadow.shadowPivot);
            LightingSystem.system.AddShadow(shadowData);
        }

        /// <summary>
        /// checks if object have a shadow, omitting shadowData getter
        /// </summary>
        /// <returns></returns>
        public bool HasShadow() => _shadowData == null;

      
        private ShadowData CreateShadowData()
        {

            ShadowData data = (ShadowData)ScriptableObject.CreateInstance(typeof(ShadowData));

            StylizedShadowCaster caster = new StylizedShadowCaster(transform, null, null, null, Vector2.zero);

            GameObject parent = caster.shadowCaster.gameObject;
            GameObject shadowGO = new GameObject(parent.name + " : shadow");
            CreatePivot(ref caster);

            caster.shadowPivot.parent = parent.transform;
            caster.shadow = shadowGO.transform;
            caster.shadow.parent = caster.shadowPivot.transform;
            shadowGO.transform.localRotation = Quaternion.identity;
            shadowGO.transform.localPosition = Vector3.zero;

            caster.shadowSr = shadowGO.AddComponent<SpriteRenderer>();
            caster.shadowSr.sortingLayerName = "Shadows";
            caster.shadowSr.sortingOrder = 1;

            caster.shadowSr.material = LightingSystem.system._shadowsMaterial;
            caster.shadowCasterSr = parent.GetComponent<SpriteRenderer>();
            caster.shadowSr.sprite = caster.shadowCasterSr.sprite;
            caster.shadow.SetGlobalScale(caster.shadowCaster.lossyScale);

            data.shadow = caster;

            /* Future
            //sprite sheet uvs
            _propBlock = new MaterialPropertyBlock();
            Vector3Int props = GetProperties(caster.shadowSr.sprite);
            if (props != Vector3Int.zero)
            {
                caster.shadowSr.GetPropertyBlock(_propBlock);
                _propBlock.SetInteger("_cellsX", props.x);
                _propBlock.SetInteger("_cellsY", props.y);
                _propBlock.SetInteger("_textureW", props.z);
                _propBlock.SetFloat("_posInGrid", 0);
                caster.shadowSr.SetPropertyBlock(_propBlock);
            }
            else 
            {
                caster.shadowSr.GetPropertyBlock(_propBlock);
                _propBlock.SetInteger("_cellsX",1);
                _propBlock.SetInteger("_cellsY", 1);
                _propBlock.SetInteger("_textureW", 1);
                _propBlock.SetFloat("_posInGrid", 0);
                caster.shadowSr.SetPropertyBlock(_propBlock);
            }*/


            return data;
        }

        /* Future
          
        private MaterialPropertyBlock _propBlock;
        private Vector3Int GetProperties(Sprite s) 
        {
            if (s.texture.width == s.rect.width && s.texture.height == s.rect.height)
                return Vector3Int.zero;

            int x = (int)(s.texture.width / s.rect.width);
            int y = (int)(s.texture.height / s.rect.height);
            int z = s.texture.width;
            return new Vector3Int(x, y, z);
        }*/

        /// <summary>
        /// creates or reuses already created pivot
        /// </summary>
        /// <param name="caster"></param>
        private void CreatePivot(ref StylizedShadowCaster caster)
        {
            for (int i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).tag == "Shadow")
                {
                    caster.shadowPivot = transform.GetChild(i);
                    return;
                }

            GameObject shadowPivot = new GameObject(caster.shadowCaster.name + " : shadowPivot");
            caster.shadowPivot = shadowPivot.transform;
            caster.shadowPivot.tag = "Shadow";

            CircleCollider2D c = caster.shadowPivot.gameObject.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
            c.radius = 0.3f;

            caster.shadowPivot.parent = caster.shadowCaster;
        }

    }


}

public static class TransformExtensionMethods
{
    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }
}