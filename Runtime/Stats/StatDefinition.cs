using System.Collections.Generic;
using UnityEngine;

namespace Dave6.StatSystem.Stat
{
    [CreateAssetMenu(fileName = "StatDefinition", menuName = "DaveAssets/StatSystem/Stat/StatDefinition")]
    public class StatDefinition : ScriptableObject
    {
        public int initialValue;
        public List<DerivedStatSource> formulaStats;
    }

    // [Serializable]
    // public enum StatKey
    // {
    //     None,
    //     // resource stat
    //     Health, Stamina, Mana,

    //     // attribute stat
    //     Strength, Agility, Intelligence,

    //     // derived stat
    //     AttackPower, AbilityPower,
    //     Armor, MagicResist,
    //     CriticalChance, CriticalDamage,
    //     AttackSpeed, MoveSpeed,
    //     HealthRegen, StaminaRegen, ManaRegen,
    // }
}
