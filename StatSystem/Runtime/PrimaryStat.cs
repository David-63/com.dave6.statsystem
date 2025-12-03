using System;
using System.Runtime.CompilerServices;
using SaveSystem;

[assembly: InternalsVisibleTo("StatSystem.Tests")]
namespace StatSystem
{
    public class PrimaryStat : Stat, ISaveable
    {
        int m_BaseValue;
        public override int baseValue { get => m_BaseValue; protected set => m_BaseValue = value; }

        public PrimaryStat(StatDefinition definition) : base(definition)
        {
            baseValue = definition.baseValue;
        }

        internal void Add(int amount)
        {
            baseValue += amount;
            CalculateValue();
        }
        internal void Substract(int amount)
        {
            baseValue -= amount;
            CalculateValue();
        }


        #region Save System

        [Serializable]
        protected class PrimaryStatData
        {
            public int baseValue;
        }
        public object data => new PrimaryStatData()
        {
            baseValue = m_BaseValue
        };
        public void Load(object data)
        {
            PrimaryStatData statData = (PrimaryStatData)data;
            m_BaseValue = statData.baseValue;
            CalculateValue();
        }


        #endregion
    }
}