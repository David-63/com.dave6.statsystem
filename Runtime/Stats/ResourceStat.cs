using System;
using System.Collections.Generic;
using Dave6.StatSystem.Effect;
using UnityEngine;

namespace Dave6.StatSystem.Stat
{
    public class ResourceStat : BaseStat, IDerived, IEffectApplicable
    {
        bool _initialFinish = false;
        float m_PreviousFinalValue;
        float m_CurrentValue;
        public float currentValue => m_CurrentValue;

        public event Action onCurrentValueChanged;
        

        List<StatReference> m_SourceStats;        
        public List<StatReference> sources => m_SourceStats;

        public ResourceStat(StatDefinition definition) : base(definition) { }
        public void SetupSources(List<StatReference> sourceStats) => m_SourceStats = sourceStats;

        public override void Initialize()
        {
            CalculateValue();
            m_PreviousFinalValue = finalValue;
            m_CurrentValue = finalValue;
            _initialFinish = true;
        }

        protected override int CalculateBaseInternal()
        {
            int totalWeight = 0;

            // total값에 각 sourceStat.finalValue * weight 더하기
            foreach (var pair in m_SourceStats)
            {
                totalWeight += (int)(pair.sourceStat.finalValue * pair.weight);
            }
            // 최종 base값 반환
            return baseValue + totalWeight;
        }

        /// <summary>
        /// Max 변경시 CurrentValue 값을 보정 해주는 함수
        /// </summary>
        protected override void AfterValueCalculated(int final)
        {
            if (!_initialFinish) return;

            // 최대치가 증가한 경우에만 값 보정
            if (m_PreviousFinalValue < final)
            {
                float ratio = m_CurrentValue / m_PreviousFinalValue;
                m_CurrentValue = final * ratio;
            }
            m_PreviousFinalValue = final;
            ClampCurrentValue(final);
        }


        public void ApplyCurrentValue(EffectDefinition effect, float value)
        {
            m_CurrentValue += value * effect.outputMultiplier;
            onCurrentValueChanged?.Invoke();
            ClampCurrentValue(finalValue);
        }
        public void ApplyCurrentPercent(EffectDefinition effect, float value)
        {
            // 현재값 *= (1 + 퍼센트)
            float delta = m_CurrentValue * value;
            m_CurrentValue += delta * effect.outputMultiplier;
            onCurrentValueChanged?.Invoke();
            ClampCurrentValue(finalValue);
        }
        public void ApplyMaxPercent(EffectDefinition effect, float value)
        {
            // 최대값 증가 → 현재값도 비례 증가시키려면 ratio 조정 필요
            float delta = finalValue * value;
            float scaledDelta = delta * effect.outputMultiplier;

            float oldMax = finalValue;
            float newMax = oldMax + scaledDelta;

            float ratio = oldMax > 0f ? m_CurrentValue / oldMax : 0f;
            m_CurrentValue = newMax * ratio;
            onCurrentValueChanged?.Invoke();
            ClampCurrentValue(newMax);
        }

        void ClampCurrentValue(float max)
        {
            m_CurrentValue = Mathf.Clamp(m_CurrentValue, 0f, max);
        }

        public void ResetCurrentValue() => m_CurrentValue = finalValue;
    }
}
