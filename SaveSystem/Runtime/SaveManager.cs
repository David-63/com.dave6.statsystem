using UnityEngine;

namespace SaveSystem
{
    // 기본 실행 순서를 1로 설정
    // 이렇게 하면 다른 스크립트의 Awake보다 먼저 실행됨
    // 라이프사이클의 모든 기능이 마지막으로 실행되는 것을 보장함
    [DefaultExecutionOrder(1)]
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] SaveData m_SaveData;

        void Awake()
        {
            if (m_SaveData.previousSaveExists)
            {
                m_SaveData.Load();
            }
        }

        void OnApplicationQuit()
        {
            m_SaveData.Save();
        }
    }
}