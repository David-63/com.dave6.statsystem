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

        

        List<SourcePair> m_SourceStats;        
        public List<SourcePair> sources => m_SourceStats;

        public ResourceStat(StatDefinition definition) : base(definition) { }
        public void SetupSources(List<SourcePair> sourceStats) => m_SourceStats = sourceStats;

        public override void Initialize()
        {
            CalculateValue();
            m_PreviousFinalValue = m_FinalValue;
            m_CurrentValue = m_FinalValue;
            _initialFinish = true;
        }

        protected override int CalculateBaseInternal()
        {
            int totalWeight = 0;

            // total값에 각 sourceStat.finalValue * weight 더하기
            foreach (var pair in m_SourceStats)
            {
                totalWeight += (int)(pair.stat.finalValue * pair.weight);
            }
            // 최종 base값 반환
            return baseValue + totalWeight;
        }

        /// <summary>
        /// Max 변경시 CurrentValue 값을 보정 해주는 함수
        /// </summary>
        protected override void AfterValueCalculated()
        {
            if (!_initialFinish) return;

            // 최대치가 증가한 경우에만 값 보정
            if (m_PreviousFinalValue < finalValue)
            {
                float ratio = m_CurrentValue / m_PreviousFinalValue;
                m_CurrentValue = finalValue * ratio;
            }
            m_PreviousFinalValue = finalValue;
            ClampCurrentValue();
        }


        public void ApplyEffect(EffectDefinition effect, float value)
        {
            switch (effect.operationType)
            {
                case EffectOperationType.Addition:
                    // 현재값 += value
                    m_CurrentValue += value * effect.outputMultiplier;
                break;
                case EffectOperationType.Subtraction:
                    // 현재값 -= value
                    m_CurrentValue -= value * effect.outputMultiplier;
                break;
                case EffectOperationType.PercentCurrentIncrease:
                    {
                        // 현재값 *= (1 + 퍼센트)
                        float delta = m_CurrentValue * value;
                        m_CurrentValue += delta * effect.outputMultiplier;
                        break;
                    }
                case EffectOperationType.PercentCurrentDecrease:
                    {
                        // 현재값 *= (1 - 퍼센트)
                        float delta = m_CurrentValue * value;
                        m_CurrentValue -= delta * effect.outputMultiplier;
                        break;
                    }
                case EffectOperationType.PercentMaxIncrease:
                    // 최대값 증가 → 현재값도 비례 증가시키려면 ratio 조정 필요
                    {
                        float delta = finalValue * value;
                        float scaledDelta = delta * effect.outputMultiplier;

                        float oldMax = finalValue;
                        float newMax = oldMax + scaledDelta;

                        float ratio = oldMax > 0f ? m_CurrentValue / oldMax : 0f;
                        m_CurrentValue = newMax * ratio;
                        break;
                    }
                case EffectOperationType.PercentMaxDecrease:
                    {
                        float delta = finalValue * value;
                        float scaledDelta = delta * effect.outputMultiplier;

                        float oldMax = finalValue;
                        float newMax = Mathf.Max(0f, oldMax - scaledDelta);

                        float ratio = oldMax > 0f ? m_CurrentValue / oldMax : 0f;
                        m_CurrentValue = newMax * ratio;
                        break;
                    }
            }
            ClampCurrentValue();
        }

        void ClampCurrentValue()
        {
            m_CurrentValue = Mathf.Clamp(m_CurrentValue, 0f, finalValue);
        }
    }
}
