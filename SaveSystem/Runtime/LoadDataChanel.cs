using System;
using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "LoadDataChannel", menuName = "DaveAssets/SaveSystem/Channel/LoadDataChannel")]
    public class LoadDataChannel : ScriptableObject
    {
        public event Action load;

        public void Load()
        {
            load?.Invoke();
        }
    }

}