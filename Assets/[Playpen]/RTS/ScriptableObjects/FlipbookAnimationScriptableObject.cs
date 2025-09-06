using UnityEngine;


[CreateAssetMenu()]
public class FlipbookAnimationScriptableObject : ScriptableObject
{
    public Mesh[] frames;
    public float frameDuration;
}
