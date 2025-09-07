using System.Collections.Generic;
using RTS;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu()]
    public class FlipbookAnimationSetScriptableObject : ScriptableObject
    {
        public List<FlipbookAnimationScriptableObject> animations;
        
        public FlipbookAnimationScriptableObject GetAnimation(FlipbookAnimationScriptableObject.AnimationType animationType)
        {
            return animations.Find(animation => animation.animationType == animationType);
        }
    }
    
}
