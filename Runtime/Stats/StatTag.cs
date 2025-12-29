using System;
using UnityEngine;

namespace Dave6.StatSystem.Stat
{
    /// <summary>
    /// 공용 스탯 태그 네이밍
    /// </summary>
    [CreateAssetMenu(fileName = "StatTag", menuName = "DaveAssets/StatSystem/Stat/StatTag")]
    public class StatTag : ScriptableObject
    {
        public string tagName;
    }
}
