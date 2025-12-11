using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using UnityEngine;

namespace ProtoCode
{
    public class ProjectileInvoker : MonoBehaviour, IStatInvoker
    {
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;

        // 원래는 플레이어가 생성하면서 초기화 함수로 본인을 넣어줘야함
        [SerializeField] IEntity m_Actor;
        [SerializeField] MonoBehaviour m_ActorMono;
        public IEntity actor => m_Actor;

        /// <summary>
        /// 생성될 때, 스텟을 받도록 할수도 있음
        /// </summary>
        public void Initialize(IEntity actorEntity)
        {
            m_Actor = actorEntity;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IStatReceiver>(out var entity))
            {
                entity.Accept(this);
            }
        }

        public void Invoke<T>(T target) where T : Component, IStatReceiver
        {
            IEntity entity = target as IEntity;
            var stat = entity.statHandler.GetHealthStat();
            Debug.Log($"Before Effect -> Health: {stat.currentValue}/{stat.finalValue}");
            
            IEntity owner = m_ActorMono as IEntity;
            owner.statHandler.ApplyEffect(effectDefinition, stat);
            Debug.Log($"After Effect -> Health: {stat.currentValue}/{stat.finalValue}");

            Destroy(gameObject);
        }
    }
}