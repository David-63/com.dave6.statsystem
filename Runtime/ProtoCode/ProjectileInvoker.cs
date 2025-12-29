using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace ProtoCode
{
    public class ProjectileInvoker : MonoBehaviour, IStatInvoker
    {
        public StatTag targetStatTag;
        public EffectDefinition effectDefinition {get;}

        // 원래는 플레이어가 생성하면서 초기화 함수로 본인을 넣어줘야함
        [SerializeField] IStatController m_Actor;
        [SerializeField] MonoBehaviour m_ActorMono;
        public IStatController actor => m_Actor;

        /// <summary>
        /// 생성될 때, 스텟을 받도록 할수도 있음
        /// </summary>
        public void Initialize(IStatController actorEntity)
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
            IStatController entity = target as IStatController;
            var isFail = entity.statHandler.TryGetStat(targetStatTag, out var health);
            if (isFail) return;
            
            IStatController owner = m_ActorMono as IStatController;
            owner.statHandler.CreateEffectInstance(effectDefinition, health);

            Destroy(gameObject);
        }
    }
}