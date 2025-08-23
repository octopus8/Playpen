using UnityEngine;

public class DOTSZombie : MonoBehaviour
{
    public class Baker : Unity.Entities.Baker<DOTSZombie>
    {
        public override void Bake(DOTSZombie authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent<Zombie>(entity);
        }
    }
}


public struct Zombie : Unity.Entities.IComponentData
{
}