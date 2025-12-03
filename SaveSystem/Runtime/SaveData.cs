using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "DaveAssets/SaveSystem/SaveData")]
    public class SaveData : ScriptableObject
    {
        [SerializeField] LoadDataChannel m_LoadDataChannel;
        [SerializeField] SaveDataChannel m_SaveDataChannel;
        [SerializeField] string m_FileName;
        [HideInInspector, SerializeField] string m_Path;

        Dictionary<string, object> m_Data = new Dictionary<string, object>();

        public bool previousSaveExists => File.Exists(m_Path);

        void OnValidate()
        {
            m_Path = Path.Combine(Application.persistentDataPath, m_FileName);
        }

        public void Save(string id, object data)
        {
            m_Data[id] = data;
        }
        public void Save()
        {
            if (previousSaveExists)
            {
                FileManager.LoadFromBinaryFile(m_Path, out m_Data);
            }
            m_SaveDataChannel.Save();
            FileManager.SaveToBinaryFile(m_Path, m_Data);
            m_Data.Clear();
        }
        public void Load(string id, out object data)
        {
            //data = m_Data.TryGetValue(id, out var value) ? value : null;            
            // 문제 확인을 위해 그대로 사용
            data = m_Data[id];
        }
        public void Load()
        {
            FileManager.LoadFromBinaryFile(m_Path, out m_Data);
            m_LoadDataChannel.Load();
            m_Data.Clear();
        }

        // 디버깅용
        [ContextMenu("Delete Save File")]
        void DeleteSave()
        {
            if (previousSaveExists)
            {
                File.Delete(m_Path);
            }
        }
    }

    /*
        GPT가 말하길

        단일 채널(m_Data)을 사용해서 처리하고, Save/Load 직후 바로 Clear는 위험하다고 함
        멀티 쓰레드/ 동시 접근을 고려하자면 lock 또는 ConcurrentDictionary 사용을 권장
    */

}