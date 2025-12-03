using System;
using System.Collections.Generic;
using LevelSystem.Nodes;
using SaveSystem;
using UnityEngine;

namespace StatSystem
{
    [RequireComponent(typeof(ILevelable))]
    public class PlayerStatController : StatController, ISaveable
    {
        protected ILevelable m_Levelable;
        protected int m_StatPoints = 5;
        public event Action statPointsChanged;

        public int statPoints
        {
            get => m_StatPoints;
            internal set
            {
                m_StatPoints = value;
                statPointsChanged?.Invoke();
            }
        }

        protected override void Awake()
        {
            m_Levelable = GetComponent<ILevelable>();
        }

        void OnEnable()
        {
            m_Levelable.initialized += OnLevelableInitialized;
            m_Levelable.willUnitialize += UnregisterEvents;
            if (m_Levelable.isInitialized)
            {
                OnLevelableInitialized();
            }
        }
        void OnDisable()
        {
            m_Levelable.initialized -= OnLevelableInitialized;
            m_Levelable.willUnitialize -= UnregisterEvents;
            if (m_Levelable.isInitialized)
            {
                UnregisterEvents();
            }
        }

        void OnLevelableInitialized()
        {
            Initialize();
            RegisterEvents();
        }

        void RegisterEvents()
        {
            m_Levelable.levelChanged += OnLevelChanged;
        }

        void UnregisterEvents()
        {
            m_Levelable.levelChanged -= OnLevelChanged;
        }

        void OnLevelChanged()
        {
            statPoints += 5;
        }

        protected override void InitializeStatFormulas()
        {
            base.InitializeStatFormulas();
            foreach (Stat currentStat in m_Stats.Values)
            {
                if (currentStat.definition.formula != null && currentStat.definition.formula.rootNode != null)
                {
                    List<LevelNode> levelNodes = currentStat.definition.formula.FindNodesOfType<LevelNode>();

                    foreach (LevelNode levelNode in levelNodes)
                    {
                        levelNode.levelable = m_Levelable;
                        m_Levelable.levelChanged += currentStat.CalculateValue;
                    }
                }
            }
        }


        #region Save System
        [Serializable]
        protected class PlayerStatControllerData : StatControllerData
        {
            public int statPoints;

            public PlayerStatControllerData(StatControllerData baseData)
            {
                this.statsData = baseData.statsData;
            }
        }

        public override object data
        {
            get
            {
                return new PlayerStatControllerData((StatControllerData)base.data)
                {
                    statPoints = m_StatPoints
                };
            }
        }

        public override void Load(object data)
        {
            base.Load(data);
            PlayerStatControllerData controllerData = (PlayerStatControllerData)data;
            m_StatPoints = controllerData.statPoints;
            statPointsChanged?.Invoke();
        }
        #endregion
    }
}
