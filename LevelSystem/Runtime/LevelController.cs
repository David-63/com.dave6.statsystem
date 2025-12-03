using System;
using System.Collections.Generic;
using Core;
using LevelSystem.Nodes;
using SaveSystem;
using UnityEngine;

namespace StatSystem
{
    public class LevelController : MonoBehaviour, ILevelable, ISaveable
    {
        [SerializeField] int m_Level = 1;
        [SerializeField] int m_CurrentExperience;
        [SerializeField] NodeGraph m_RequireExperienceFormula;

        bool m_IsInitialized;

        public int level => m_Level;
        public event Action levelChanged;
        public event Action currentExperienceChanged;

        public int currentExperience
        {
            get => m_CurrentExperience;
            set
            {
                if (value >= requiredExperience)
                {
                    m_CurrentExperience = value - requiredExperience;
                    currentExperienceChanged?.Invoke();
                    m_Level++;
                    levelChanged?.Invoke();
                }
                else if (value < requiredExperience)
                {
                    m_CurrentExperience = value;
                    currentExperienceChanged?.Invoke();
                }
            }
        }

        public int requiredExperience => Mathf.RoundToInt(m_RequireExperienceFormula.rootNode.value);
        public bool isInitialized => m_IsInitialized;

        public event Action initialized;
        public event Action willUnitialize;

        void Awake()
        {
            if (!m_IsInitialized)
            {
                Initialize();
            }
        }

        void Initialize()
        {
            List<LevelNode> levelNodes = m_RequireExperienceFormula.FindNodesOfType<LevelNode>();
            foreach (var node in levelNodes)
            {
                node.levelable = this;
            }

            m_IsInitialized = true;
            initialized?.Invoke();
        }

        void OnDestroy()
        {
            willUnitialize?.Invoke();
        }

        #region Save System
        [Serializable]
        protected class LevelControllerData
        {
            public int level;
            public int currentExperience;
        }
        public object data => new LevelControllerData()
        {
            level = m_Level,
            currentExperience = m_CurrentExperience
        };
        public void Load(object data)
        {
            LevelControllerData levelData = (LevelControllerData)data;
            m_Level = levelData.level;
            m_CurrentExperience = levelData.currentExperience;
            currentExperienceChanged?.Invoke();
            levelChanged?.Invoke();
        }
        #endregion
    }
}