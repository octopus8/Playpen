using UnityEngine;


[CreateAssetMenu()]
public class FlipbookAnimationScriptableObject : ScriptableObject
{
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
    
    public AnimationType animationType;
    public Mesh[] frames;
    public float frameDuration;
}
