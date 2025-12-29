using System;
using System.Collections.Generic;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Stat;

namespace Dave6.StatSystem.Stat
{
    public interface IDerived
    {
        List<StatReference> sources { get; }

        /// <summary>
        /// Definition 에 담긴 formulaStat의 타입과 일치하는 인스턴스 스텟 리스트를 연결
        /// </summary>
        void SetupSources(List<StatReference> sources);
    }

    public interface IEffectApplicable
    {
        void ApplyCurrentValue(EffectDefinition effect, float value);
        void ApplyCurrentPercent(EffectDefinition effect, float value);
        void ApplyMaxPercent(EffectDefinition effect, float value);
    }

    /// <summary>
    /// DB에서 사용하는 매핑 클래스
    /// </summary>
    [Serializable]
    public class StatBindTag
    {
        public StatTag statTag;
        public StatDefinition statDefinition;
    }

    /// <summary>
    /// 참조할 스텟의 key와 weight 정보가 담겨있음
    /// </summary>
    [Serializable]
    public struct DerivedStatSource
    {
        public StatTag key;  // 어떤 스탯에 의존?
        public float weight;         // 얼마나 영향을 주는지
    }

    public readonly struct StatReference
    {
        public readonly BaseStat sourceStat;
        public readonly float weight;

        public StatReference(BaseStat sourceStat, float weight)
        {
            this.sourceStat = sourceStat;
            this.weight = weight;
        }
    }

    /// <summary>
    /// 참조할 스텟의 key와 weight 정보가 담겨있음
    /// </summary>
    public class EffectPreset
    {
        public readonly IReadOnlyList<StatReference> sources;

        public EffectPreset(IEnumerable<StatReference> sources)
        {
            this.sources = new List<StatReference>(sources);
        }
    }

}
