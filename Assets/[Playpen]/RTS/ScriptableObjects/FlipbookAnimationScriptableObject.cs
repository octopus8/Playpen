using UnityEngine;


[CreateAssetMenu()]
public class FlipbookAnimationScriptableObject : ScriptableObject
{
    public enum AnimationType
    {
        SoldierNone,
        SoldierIdle,
        SoldierWalk,
    }
    
    public AnimationType animationType;
    public Mesh[] frames;
    public float frameDuration;
}
