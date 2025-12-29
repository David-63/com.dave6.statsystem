using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace ProtoCode
{
    public class PickupInvoker : MonoBehaviour, IStatInvoker
    {
        [SerializeField] StatTag m_TargetStatTag;
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IStatReceiver>(out var entity))
            {
                entity.Accept(this);
            }
        }

        public void Invoke<T>(T target) where T : Component, IStatReceiver
        {
            IStatController entity = target as IStatController;
            var isFail = entity.statHandler.TryGetStat(m_TargetStatTag, out var health);
            if (isFail) return;
            
            entity.statHandler.CreateEffectInstance(effectDefinition, health);

            Destroy(gameObject);
        }
    }
}