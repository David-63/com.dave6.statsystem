using System.Collections.Generic;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.StatSystem
{
    /// <summary>
    /// 스텟 시스템을 사용하는 컨트롤러가 인터페이스를 구현
    /// </summary>
    
    public interface IEntity
    {
        StatDatabase statDatabase { get; }
        StatHandler statHandler { get; }

        public ResourceStat health { get; set; }

        void InitializeStat();

        // visitor 패턴을 사용해서 상호작용..?
    }

    /// <summary>
    /// 캐릭터의 전반적인 스텟 핸들링 제공
    /// 1. 목표 스텟 찾기
    /// 2. 이팩트 생성
    /// ..?
    /// </summary>
    public class StatHandler
    {
        //[SerializeField] StatDatabase m_StatDatabase;
        StatDatabase m_StatDatabase;

        // StatDatabase SO를 키값으로 씀
        protected Dictionary<StatDefinition, BaseStat> m_Stats = new();
        public Dictionary<StatDefinition, BaseStat> stats => m_Stats;

        readonly Dictionary<EffectDefinition, EffectPreset> m_cachedEffects = new();

        // 채력 스텟 캐싱
        ResourceStat m_Health;


        public StatHandler(StatDatabase statDatabase)
        {
            m_StatDatabase = statDatabase;
        }

        public void InitializeStat()
        {
            // 스텟 생성
            foreach (StatDefinition definition in m_StatDatabase.attributes)
            {
                m_Stats.Add(definition, new Attribute(definition));
            }

            foreach (StatDefinition definition in m_StatDatabase.secondaryStat)
            {
                m_Stats.Add(definition, new SecondaryStat(definition));
            }

            foreach (StatDefinition definition in m_StatDatabase.resourceStat)
            {
                m_Stats.Add(definition, new ResourceStat(definition));
            }

            // Formula 적용
            foreach (var stat in m_Stats.Values)
            {
                // formula를 사용하는 타입의 스텟 (Secondary Stat)
                if (stat is IDerived derivedStat)
                {
                    // source를 등록해줘야함
                    List<SourcePair> sources = new();

                    foreach (var target in stat.definition.formulaStats)
                    {
                        sources.Add(new(m_Stats[target.key], target.value));
                    }

                    derivedStat.SetupSources(sources);
                }
            }

            // 초기화 진행
            foreach (var stat in m_Stats.Values)
            {
                stat.Initialize();
            }
        }


        public BaseStat GetStat(string name)
        {
            foreach (var pair in m_Stats)
            {
                if (pair.Key.name == name)
                {
                    return pair.Value;
                }
            }
            return null;
        }
        public BaseStat GetStat(StatDefinition name)
        {
            foreach (var pair in m_Stats)
            {
                if (pair.Key == name)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        // 차나리 주요 스텟을 스크립트로 구성해도 괜찬을듯?
        public ResourceStat GetHealthStat()
        {
            if (m_Health != null)
            {
                Debug.Log($"{m_Health}");
                return m_Health;
            }
            else
            {
                foreach (var kv in m_Stats)
                {
                    if (kv.Key.name.Contains("R_Health"))
                    {
                        m_Health = kv.Value as ResourceStat;
                        Debug.Log($"{m_Health}");
                        return m_Health;
                    }
                }
            }

            Debug.Log("Health가 없거나 네이밍이 'R_Health' 와 다름 ");

            return null;
        }

        public void ApplyEffect(EffectDefinition definition, BaseStat target)
        {
            EffectPreset effectPreset = GetOrCreateEffectPreset(definition);

            new EffectInstance(definition, target, effectPreset).Apply();
        }

        EffectPreset GetOrCreateEffectPreset(EffectDefinition definition)
        {
            if (!m_cachedEffects.TryGetValue(definition, out var sources))
            {
                sources = BuildEffectPreset(definition);
                m_cachedEffects.Add(definition, sources);
            }
            return sources;
        }

        EffectPreset BuildEffectPreset(EffectDefinition definition)
        {
            var list = new List<SourcePair>();

            foreach (var tuple in definition.sourceStats)
            {
                var stat = GetStat(tuple.key.name);
                if (stat != null)
                {
                    list.Add(new SourcePair(stat, tuple.value));
                }
            }

            return new EffectPreset(list);
        }


    }
}
