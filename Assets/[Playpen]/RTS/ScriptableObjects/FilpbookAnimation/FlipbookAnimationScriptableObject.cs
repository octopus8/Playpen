using UnityEngine;

namespace RTS
{
    
    /// <summary>
    /// A scriptable object that holds data for a flipbook animation.
    /// </summary>
    [CreateAssetMenu()]
    public class FlipbookAnimationScriptableObject : ScriptableObject
    {
        /// <summary> The type of animation. </summary>
        public enum AnimationType
        {
            None,
            SoldierIdle,
            SoldierWalk,
            SoldierAim,
            SoldierShoot,
            ZombieIdle,
            ZombieWalk,
            ZombieMeleeAttack,
            ScoutIdle,
            ScoutWalk,
            ScoutAim,
            ScoutShoot,
        }
    
        /// <summary> The type of animation. </summary>
        [Tooltip("The type of animation.")]
        public AnimationType animationType;
    
        /// <summary> The frames of the animation. </summary>
        [Tooltip("The frames of the animation.")]
        public Mesh[] frames;
    
        /// <summary> The duration of each frame in seconds. </summary>
        [Tooltip("The duration of each frame in seconds.")]
        public float frameDuration;

        
        /// <summary>
        /// Determines if the given animation type is a one-shot animation.
        /// One-shot animations play once and then stop, while looping animations repeat indefinitely.
        /// </summary>
        public static bool IsAnimationOneShot(AnimationType animationType)
        {
            return animationType == AnimationType.SoldierShoot ||
                   animationType == AnimationType.ZombieMeleeAttack ||
                   animationType == AnimationType.ScoutShoot;
        }
    }
}

