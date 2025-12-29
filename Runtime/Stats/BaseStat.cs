using System;
using System.Collections.Generic;
using Dave6.StatSystem.Effect;
using UnityEngine;

namespace Dave6.StatSystem.Stat
{
    public abstract class BaseStat
    {
        public StatDefinition definition { get; private set; }
        public int baseValue { get; private set; }

        /// <summary>
        /// UI, 애니메이션, 사운드, 효과, 기타 시스템에 이벤트 전달
        /// </summary>
        public event Action onValueChanged;

        #region Dirty flag 패턴
        bool isDirty = true;
        int m_FinalValue;
        public int finalValue
        {
            get
            {
                if (isDirty) CalculateValue();
                return m_FinalValue;
            }
            private set => m_FinalValue = value;
        }

        public void MarkDirty()
        {
            if (isDirty) return;
            isDirty = true;
            onValueChanged?.Invoke();
        }
        #endregion


        protected List<BaseContribution> m_BaseContributions = new();

        public BaseStat(StatDefinition definition)
        {
            this.definition = definition;
            baseValue = definition.initialValue;
            finalValue = definition.initialValue;
        }

        public virtual void Initialize()
        {
            CalculateValue();
        }

        /// <summary>
        /// 1) base 값이 변하거나, 2) modifier 요소가 변경되면 계산
        /// </summary>
        public void CalculateValue()
        {
            int calcBase = CalculateBaseInternal();
            CalculateFinalValue(calcBase);
            AfterValueCalculated(m_FinalValue);
            isDirty = false;
        }

        /// <summary>
        /// abstract로 할지 고민됬었는데 그냥 필요한쪽이 구현하는거로 결정
        /// </summary>
        /// <returns></returns>
        protected virtual int CalculateBaseInternal()
        {
            return baseValue;
        }

        void CalculateFinalValue(int calcBase)
        {
            float flat = 0;
            float percent = 0f;             // 합연산
            float finalMultiplier = 1f;     // 곱연산

            foreach (var m in m_BaseContributions)
            {
                (flat, percent, finalMultiplier) = m.valueType switch
                {
                    StatValueType.Flat              => (flat + m.magnitude, percent, finalMultiplier),
                    StatValueType.Percent           => (flat, percent + m.magnitude * 0.01f, finalMultiplier),
                    StatValueType.finalMultiplier   => (flat, percent, finalMultiplier * (1f + m.magnitude * 0.01f)),
                    _ => throw new NotSupportedException()
                };
            }

            // 플렛 적용
            int result = Mathf.FloorToInt(calcBase + flat);

            // 퍼센트 적용
            result = Mathf.FloorToInt(result * (1f + percent));

            // 최종 배율 적용
            finalValue = Mathf.FloorToInt(result * finalMultiplier);
        }

        /// <summary>
        /// CurrentValue 계산이 필요한 경우에 구현
        /// </summary>
        protected virtual void AfterValueCalculated(int final) { }

        public void AddBaseContribution(BaseContribution contribution)
        {
            MarkDirty();
            m_BaseContributions.Add(contribution);
            onValueChanged?.Invoke();
            CalculateValue();
        }
        
        public void RemoveBaseContribution(object source)
        {
            MarkDirty();
            m_BaseContributions.RemoveAll(c => c.source == source);
            onValueChanged?.Invoke();
            CalculateValue();
        }
    }
}
