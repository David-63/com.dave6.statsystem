using System;
using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveDataChannel", menuName = "DaveAssets/SaveSystem/Channel/SaveDataChannel")]
    public class SaveDataChannel : ScriptableObject
    {
        public event Action save;

        public void Save()
        {
            save?.Invoke();
        }
    }

}