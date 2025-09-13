using UnityEngine;


[CreateAssetMenu()]
public class FlipbookAnimationScriptableObject : ScriptableObject
{
    /// <summary> The type of animation. </summary>
    [Tooltip("The type of animation.")]
    public enum AnimationType
    {
        None,
        SoldierIdle,
        SoldierWalk,
        SoldierAim,
        SoldierShoot,
        ZombieIdle,
        ZombieWalk,
        ZombieMeleeAttack
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
}
