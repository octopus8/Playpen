using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Contains the set of flipbook animations for all entities.
    /// </summary>
    [CreateAssetMenu()]
    public class FlipbookAnimationSetScriptableObject : ScriptableObject
    {
        /// <summary> The list of all flipbook animations. </summary>
        [Tooltip("The list of all flipbook animations.")]
        [SerializeField]
        private List<FlipbookAnimationScriptableObject> animations;
        
        
        /// <summary>
        /// Get the animation corresponding to the given animation type.
        /// </summary>
        public FlipbookAnimationScriptableObject GetAnimation(FlipbookAnimationScriptableObject.AnimationType animationType)
        {
            return animations.Find(animation => animation.animationType == animationType);
        }
    }
    
}
