using System;
using System.Collections.Generic;
using SaveSystem;
using StatSystem.Nodes;
using UnityEngine;

namespace StatSystem
{
    public class StatController : MonoBehaviour, ISaveable
    {
        [SerializeField] StatDatabase m_StatDatabase;
        protected Dictionary<string, Stat> m_Stats = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, Stat> stats => m_Stats;

        bool m_IsInitialized;
        public bool isInitialized => m_IsInitialized;
        public event Action Initialized;
        public event Action WillUninitialize;

        protected virtual void Awake()
        {
            if (!m_IsInitialized)
            {
                Initialize();
            }
        }

        void OnDestroy()
        {
            WillUninitialize?.Invoke();
        }

        protected void Initialize()
        {
            foreach (StatDefinition definition in m_StatDatabase.stats)
            {
                m_Stats.Add(definition.name, new Stat(definition));
            }

            foreach (StatDefinition definition in m_StatDatabase.primaryStats)
            {
                m_Stats.Add(definition.name, new PrimaryStat(definition));
            }
            
            foreach (StatDefinition definition in m_StatDatabase.attributes)
            {
                m_Stats.Add(definition.name, new Attribute(definition));
            }

            InitializeStatFormulas();

            foreach (Stat stat in m_Stats.Values)
            {
                stat.Initialize();
            }
            
            m_IsInitialized = true;
            Initialized?.Invoke();
        }

        protected virtual void InitializeStatFormulas()
        {
            foreach (Stat currentStat in m_Stats.Values)
            {
                if (currentStat.definition.formula != null && currentStat.definition.formula.rootNode != null)
                {
                    List<StatNode> statNodes = currentStat.definition.formula.FindNodesOfType<StatNode>();

                    foreach (StatNode statNode in statNodes)
                    {
                        if (m_Stats.TryGetValue(statNode.statName.Trim(), out Stat stat))
                        {
                            statNode.stat = stat;
                            stat.valueChanged += currentStat.CalculateValue;
                        }
                        else
                        {
                            Debug.LogWarning($"Stat {statNode.statName.Trim()} does not exist!");
                        }
                    }
                }
            }
        }

        #region Save System
        [Serializable]
        protected class StatControllerData
        {
            public Dictionary<string, object> statsData;
        }
        public virtual object data
        {
            get
            {
                Dictionary<string, object> stats = new();
                foreach (var stat in m_Stats.Values)
                {
                    if (stat is ISaveable saveableStat)
                    {
                        stats.Add(stat.definition.name, saveableStat.data);
                    }
                }

                return new StatControllerData()
                {
                    statsData = stats
                };
            }
        }
        public virtual void Load(object data)
        {
            StatControllerData controllerData = (StatControllerData)data;
            foreach (var stat in m_Stats.Values)
            {
                if (stat is ISaveable saveableStat)
                {
                    saveableStat.Load(controllerData.statsData[stat.definition.name]);
                }
            }
        }
        #endregion
    }
}