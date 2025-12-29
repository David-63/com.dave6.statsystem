using System.Collections.Generic;
using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using Unity.Collections;
using UnityEngine;

namespace ProtoCode
{
    public class StatController : MonoBehaviour, IStatController, IStatReceiver
    {
        [SerializeField] StatDatabase m_StatDatabase;
        public StatDatabase statDatabase => m_StatDatabase;

        StatHandler m_StatHandler;
        public StatHandler statHandler => m_StatHandler;

        public ResourceStat myHealth { get; set; }


        // 디버깅 이후에 사리질 수 있음
        //[SerializeField] EffectDefinition testEffect;


        void Awake()
        {
            Init_StatHandler();
        }

        public void Init_StatHandler()
        {
            m_StatHandler = new StatHandler(m_StatDatabase);
            m_StatHandler.InitializeStat();
        }

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }

        public void CheckHealth()
        {
            
        }
    }
}