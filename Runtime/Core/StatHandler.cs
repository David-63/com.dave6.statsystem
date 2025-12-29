using System.Collections.Generic;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.StatSystem
{
    /// <summary>
    /// 스텟 시스템을 사용하는 컨트롤러가 인터페이스를 구현
    /// </summary>
    
    public interface IStatController
    {
        StatDatabase statDatabase { get; }
        StatHandler statHandler { get; }

        public ResourceStat myHealth { get; set; }

        void Init_StatHandler();

        void CheckHealth();

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
        StatDatabase m_StatDatabase;

        public Dictionary<StatTag, BaseStat> statStorage = new();

        readonly Dictionary<EffectDefinition, EffectPreset> m_CachedEffectPresets = new();

        List<EffectInstance> m_AppliedEffects = new();

        public StatHandler(StatDatabase statDatabase)
        {
            m_StatDatabase = statDatabase;
        }

        public void InitializeStat()
        {
            // 스탯 생성
            foreach (StatBindTag stat in m_StatDatabase.attributeStatTags)
            {
                statStorage.Add(stat.statTag, new Attribute(stat.statDefinition));
            }

            foreach (StatBindTag stat in m_StatDatabase.secondaryStatTags)
            {
                statStorage.Add(stat.statTag, new SecondaryStat(stat.statDefinition));
            }

            foreach (StatBindTag stat in m_StatDatabase.resourceStatTags)
            {
                statStorage.Add(stat.statTag, new ResourceStat(stat.statDefinition));
            }

            // formula 적용
            foreach (var stat in statStorage.Values)
            {
                // formula를 사용하는 타입의 스텟 (Secondary Stat)
                if (stat is IDerived derivedStat)
                {
                    // source를 등록해줘야함
                    List<StatReference> sources = new();

                    foreach (var target in stat.definition.formulaStats)
                    {
                        sources.Add(new(statStorage[target.key], target.weight));
                    }

                    derivedStat.SetupSources(sources);
                }
            }

            // 초기화 진행
            foreach (var stat in statStorage.Values)
            {
                stat.Initialize();
            }
        }

        public void OnUpdate()
        {
            var toRemove = new List<EffectInstance>();

            foreach (var effect in m_AppliedEffects)
            {
                effect.OnUpdate();
                if (effect.disposed)
                {
                    toRemove.Add(effect);
                }
            }

            foreach (var effect in toRemove)
            {
                effect.Cleanup();
                m_AppliedEffects.Remove(effect);
            }
        }

        public bool TryGetStat(StatTag key, out BaseStat stat)
        {
            return statStorage.TryGetValue(key, out stat);
        }

        public EffectInstance CreateEffectInstance(EffectDefinition definition, BaseStat target)
        {
            EffectPreset effectPreset = GetOrCreateEffectPreset(definition);
            var effect = new EffectInstance(definition, target, effectPreset);
            m_AppliedEffects.Add(effect);

            if (EffectApplyMode.Sustained == definition.applyMode)
            {
                effect.ApplySustained();
            }
            else
            {
                effect.ApplyInstant();
            }

            return effect;
        }

        EffectPreset GetOrCreateEffectPreset(EffectDefinition definition)
        {
            if (!m_CachedEffectPresets.TryGetValue(definition, out var sources))
            {
                sources = BuildEffectPreset(definition);
                m_CachedEffectPresets.Add(definition, sources);
            }
            return sources;
        }

        EffectPreset BuildEffectPreset(EffectDefinition definition)
        {
            var list = new List<StatReference>();

            foreach (var tuple in definition.sources)
            {
                if (statStorage.TryGetValue(tuple.key, out var stat))
                {
                    list.Add(new StatReference(stat, tuple.weight));
                }
            }

            return new EffectPreset(list);
        }

        public void AddBaseContribution(StatTag targetTag, BaseContribution contribution)
        {
            TryGetStat(targetTag, out var target);
            target?.AddBaseContribution(contribution);
        }


        public void RemoveBaseContribution(StatTag targetTag, object source)
        {
            TryGetStat(targetTag, out var target);
            target?.RemoveBaseContribution(source);
        }


    }
}
