using System.Collections.Generic;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.StatSystem
{
    /*
        스텟 리스트는 계층적으로 존재해야함
        
    */
    [CreateAssetMenu(fileName = "StatDatabase", menuName = "DaveAssets/StatSystem/Stat/StatDatabase")]
    public class StatDatabase : ScriptableObject
    {
        public List<StatBindTag> attributeStatTags = new();
        public List<StatBindTag> secondaryStatTags = new();
        public List<StatBindTag> resourceStatTags = new();
    }
}
