using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace ProtoCode
{
    public class HitCollider : MonoBehaviour, IStatInvoker
    {
        [SerializeField] StatTag m_TargetStatTag;
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;

        // 원래는 플레이어가 생성하면서 초기화 함수로 본인을 넣어줘야함
        [SerializeField] IStatController m_Actor;
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
            var isFail = entity.statHandler.TryGetStat(m_TargetStatTag, out var health);
            if (isFail) return;

            //m_Actor.statHandler.ApplyInstantEffect(effectDefinition, stat);
            m_Actor.statHandler.CreateEffectInstance(effectDefinition, health);
        }
    }
}