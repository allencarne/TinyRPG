using UnityEngine;
using UnityEditor;

namespace Modern2D
{

    [CanEditMultipleObjects]
    public class SetLayerAfterDemoSceneLoads : MonoBehaviour
    {
        [SerializeField] public string layer = "";
    }

}