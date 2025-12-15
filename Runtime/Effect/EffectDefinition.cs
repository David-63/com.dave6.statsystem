using System;
using System.Collections.Generic;
using UnityEngine;
using Dave6.StatSystem.Stat;

namespace Dave6.StatSystem.Effect
{
    [Serializable]
    public enum EffectOperationType
    {
        Addition,
        Subtraction,
        PercentCurrentIncrease,
        PercentCurrentDecrease,
        PercentMaxIncrease,
        PercentMaxDecrease,
    }

    [Serializable]
    public enum EffectApplyMode
    {
        Instant,                // 즉시 1회 적용
        Periodic,               // 일정 시간 동안 주기?적으로 적용
        Sustained,              // 버프 디버프처럼 지속시간동안 단 한번 적용됨
        // Stackable,              // 누적되는 방식, 다른 타입에 추가되는 플래그 개념¿
    }
    /*
    ExcutionMode, StackMode 이렇게 나누는것도 고려해볼만 할듯
    */
    [Serializable]
    public struct EffectFormulaSource
    {
        public StatDefinition key;  // 어떤 스탯에 의존?
        public float value;         // 얼마나 영향을 주는지
    }

    [CreateAssetMenu(fileName = "EffectDefinition", menuName = "DaveAssets/StatSystem/Effect")]
    public class EffectDefinition : ScriptableObject
    {
        // 연산식
        [SerializeField] EffectOperationType m_OperationType;
        public EffectOperationType operationType => m_OperationType;

        // 적용방식(아직 사용 안함, Strategy 패턴으로 구현할듯?)
        [SerializeField] EffectApplyMode m_ApplyMode;
        public EffectApplyMode applyMode => m_ApplyMode;

        // 전달값 방식 (BaseValueType) | Stat Based, Flat Based 이렇게 구현할수도 있음
        [SerializeField] float m_FlatValue;
        public float flatValue => m_FlatValue;
        // 참조 속성
        [SerializeField] List<EffectFormulaSource> m_SourceStats;
        public List<EffectFormulaSource> sourceStats => m_SourceStats;
        // 배율 추가
        [SerializeField] float m_OutputMultiplier = 1f;
        public float outputMultiplier => m_OutputMultiplier;

        // 유지시간
        [SerializeField] float m_Duration = -1;
        public float duration => m_Duration;
    }
}
