using Unity.Entities;
using UnityEngine;

public class DOTSUnit : MonoBehaviour
{
    [SerializeField]
    private Faction faction;
    
    public class Baker : Baker<DOTSUnit>
    {
        public override void Bake(DOTSUnit authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit
            {
                faction = authoring.faction,
            });
        }
    }
}

public struct Unit : IComponentData
{
    public Faction faction;
}
