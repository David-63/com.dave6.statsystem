using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class SaveController : MonoBehaviour
    {
        [SerializeField] SaveData m_SaveData;
        [SerializeField] SaveDataChannel m_SaveDataChannel;
        [SerializeField] LoadDataChannel m_LoadDataChannel;
        [HideInInspector, SerializeField] string m_Id;

        void Reset()
        {
            m_Id = Guid.NewGuid().ToString();
        }

        void OnEnable()
        {
            m_LoadDataChannel.load += OnLoadData;
            m_SaveDataChannel.save += OnSaveData;
        }

        void OnDisable()
        {
            m_LoadDataChannel.load -= OnLoadData;
            m_SaveDataChannel.save -= OnSaveData;
        }
        void OnSaveData()
        {
            Dictionary<string, object> data = new();
            foreach (var saveable in GetComponents<ISaveable>())
            {
                data[saveable.GetType().ToString()] = saveable.data;
            }
            m_SaveData.Save(m_Id, data);
        }
        void OnLoadData()
        {
            m_SaveData.Load(m_Id, out object data);
            Dictionary<string, object> dictionary = data as Dictionary<string, object>;
            foreach (var saveable in GetComponents<ISaveable>())
            {
                // if (dictionary.TryGetValue(saveable.GetType().ToString(), out object saveableData))
                // {
                //     saveable.Load(saveableData);
                // }
                saveable.Load(dictionary[saveable.GetType().ToString()]);
            }
        }


    }
}