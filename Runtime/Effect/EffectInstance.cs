using System;
using Dave6.StatSystem.Stat;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.StatSystem.Effect
{
    /// <summary>
    /// 얼마를 적용할지 계산, 어디에 적용할지 정함
    /// </summary>
    public class EffectInstance : IDisposable
    {
        public EffectDefinition definition { get; private set; }
        public BaseStat targetStat { get; private set; }
        public EffectPreset effectPresets { get; private set; }

        BaseContribution m_Contribution; // nullable

        Timer m_TimeRemaining;
        float dotDelay = 0.1f;
        float currentTime = 0;

        public bool disposed { get; private set; }

        /// <summary>
        /// 1)Effect 정의  2)대상 스텟  3)빠른참조¿
        /// </summary>
        public EffectInstance(EffectDefinition definition, BaseStat target, EffectPreset sources)
        {
            this.definition = definition;
            targetStat = target;
            effectPresets = sources;
            if (definition.duration != -1)
            {
                m_TimeRemaining = new Countdown(definition.duration);
                m_TimeRemaining.OnTimerStop += Dispose;
                m_TimeRemaining.Start();
            }
            //InitializeEffectValue();
        }

        int Evaluate()
        {
            float total = definition.flatValue;

            foreach (var pair in effectPresets.sources)
            {
                total += pair.sourceStat.finalValue * pair.weight;
            }
            return (int)total;
        }

        public void InitializeEffectValue()
        {
            float totalWeight = 0;

            // total값에 각 sourceStat.finalValue * weight 더하기
            foreach (var pair in effectPresets.sources)
            {
                totalWeight += pair.sourceStat.finalValue * pair.weight;
            }
            // 최종 base값 반환
            //m_TotalValue = totalWeight + definition.flatValue;
        }

        public void OnUpdate()
        {
            if (EffectApplyMode.Periodic != definition.applyMode) return;
            currentTime += Time.deltaTime;

            if (currentTime >= dotDelay)
            {
                ApplyInstant();
                currentTime = 0;
            }
        }

        public virtual void ApplyInstant()
        {
            if (targetStat is IEffectApplicable applicable)
            {
                switch (definition.instant.operationType)
                {
                    case ValueOperationType.Current:
                    applicable.ApplyCurrentValue(definition, Evaluate());
                    break;
                    case ValueOperationType.CurrentPercent:
                    applicable.ApplyCurrentPercent(definition, Evaluate());
                    break;
                    case ValueOperationType.MaxPercent:
                    applicable.ApplyMaxPercent(definition, Evaluate());
                    break;
                }
            }
        }
        public virtual void ApplySustained()
        {
            m_Contribution = new BaseContribution(definition.sustained.valueType, Evaluate(), this);
            targetStat.AddBaseContribution(m_Contribution);
        }

        public void Dispose()
        {
            disposed = true;
        }

        public void Cleanup()
        {
            if (m_Contribution != null)
            {
                targetStat.RemoveBaseContribution(m_Contribution);
                m_Contribution = null;
            }
        }
    }
}
