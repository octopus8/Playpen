using UnityEngine;

public class DOTSFindTarget : MonoBehaviour
{
    [SerializeField]
    private float range = 5f;

    [SerializeField]
    private Faction targetFaction;
    
    [SerializeField]
    private float maxTimerSeconds = 0.2f;
    
    public class Baker : Unity.Entities.Baker<DOTSFindTarget>
    {
        public override void Bake(DOTSFindTarget authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new FindTarget
            {
                range = authoring.range,
                targetFaction = authoring.targetFaction,
                maxTimer = authoring.maxTimerSeconds,
            });
        }
    }
}


public struct FindTarget : Unity.Entities.IComponentData
{
    public float range;
    public Faction targetFaction;
    public float timer;
    public float maxTimer;
}
