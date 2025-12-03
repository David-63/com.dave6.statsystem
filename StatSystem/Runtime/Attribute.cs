using System;
using SaveSystem;
using UnityEngine;

namespace StatSystem
{
    public class Attribute : Stat, ISaveable
    {
        protected int m_CurrentValue;
        public int currentValue { get => m_CurrentValue; private set => m_CurrentValue = value; }


        public event System.Action currentValueChanged;
        public event System.Action<StatModifier> appliedModifier;
        
        public Attribute(StatDefinition definition) : base(definition)
        {
            currentValue = baseValue;
        }

        public virtual void ApplyModifier(StatModifier modifier)
        {
            int nextValue = currentValue;

            switch (modifier.Type)
            {
                case ModifierOperationType.Additive:
                nextValue += modifier.Magnitude;
                break;

                case ModifierOperationType.Multiplicative:
                nextValue *= modifier.Magnitude;
                break;
                case ModifierOperationType.Override:
                nextValue = modifier.Magnitude;
                break;
            }
            nextValue = Mathf.Clamp(nextValue, 0, value);

            if (currentValue != nextValue)
            {
                currentValue = nextValue;
                currentValueChanged?.Invoke();
                appliedModifier?.Invoke(modifier);
            }
        }

        #region Save System
        [Serializable]
        protected class AttributeData
        {
            public int currentValue;
        }

        public object data => new AttributeData()
        {
            currentValue = m_CurrentValue
        };


        public void Load(object data)
        {
            AttributeData attributeData = (AttributeData)data;
            m_CurrentValue = attributeData.currentValue;
            currentValueChanged?.Invoke();
        }

        #endregion

    }
}