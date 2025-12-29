using System;
using System.Collections.Generic;
using UnityEngine;
using Dave6.StatSystem.Stat;

namespace Dave6.StatSystem.Effect
{

    #region Effect Enum 정의
    [Serializable]
    public enum ValueOperationType
    {
        Current,
        CurrentPercent,
        MaxPercent,
    }

//==========================================================================

    [Serializable]
    public enum SourceType
    {
        Owner,      // 시전자, 공격자
        Static      // 맵, 트랩, 아이템
    }


//==========================================================================

    [Serializable]
    public enum EffectApplyMode
    {
        Instant,                // 즉시 1회 적용
        Periodic,             // 반복 적용        (지속피해 지속회복)
        Sustained,              // 1회 적용 및 유지     (버프 디버프)
    }

    [Serializable]
    public class InstantPayload
    {
        public ValueOperationType operationType;
    }
    
    [Serializable]
    public class PeriodicPayload
    {
        public ValueOperationType operationType;
        public float tickInterval = 0.1f;
    }

    [Serializable]
    public class SustainedPayload
    {
        public StatValueType valueType;
    }

    #endregion

    [CreateAssetMenu(fileName = "EffectDefinition", menuName = "DaveAssets/StatSystem/Effect/EffectDefinition")]
    public class EffectDefinition : ScriptableObject
    {
        public SourceType sourceType;
        public float flatValue;
        public List<DerivedStatSource> sources;

        public EffectApplyMode applyMode;
        public InstantPayload instant;
        public SustainedPayload sustained;
        public PeriodicPayload periodic;

        public float duration = 1;
        public float outputMultiplier = 1;

    }

    #region 아직 안씀
    [Serializable]
    public enum EffectStackMode
    {
        None,                   // 중첩 불가
        RefreshDuration,        // 지속시간만 갱신
        StackValue,             // 값 누적
        Replace,                // 기존 효과 교체
    }
    #endregion
}
