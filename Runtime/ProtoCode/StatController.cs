using System.Collections.Generic;
using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using Unity.Collections;
using UnityEngine;

namespace ProtoCode
{
    public class StatController : MonoBehaviour, IEntity, IStatReceiver
    {
        [SerializeField] StatDatabase m_StatDatabase;
        public StatDatabase statDatabase => m_StatDatabase;

        StatHandler m_StatHandler;
        public StatHandler statHandler => m_StatHandler;

        public ResourceStat health { get; set; }


        // 디버깅 이후에 사리질 수 있음
        [SerializeField] EffectDefinition testEffect;


        void Awake()
        {
            Debug.Log($"스텟 초기화 진행");
            InitializeStat();
        }

        public void InitializeStat()
        {
            m_StatHandler = new StatHandler(m_StatDatabase);
            m_StatHandler.InitializeStat();

            // 스텟 테스트
            foreach (var stat in m_StatHandler.stats)
            {
                Debug.Log($"{stat.Key}");
            }

        }

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
    }
}