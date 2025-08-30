using Unity.Entities;
using UnityEngine;

public class DOTSHealth : MonoBehaviour
{
    public int healthAmount = 100;

    public class Baker : Baker<DOTSHealth>
    {
        public override void Bake(DOTSHealth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health
            {
                healthAmount = authoring.healthAmount
            });
        }
    }
}


public struct Health : IComponentData
{
    public int healthAmount;
}