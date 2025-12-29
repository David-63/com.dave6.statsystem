using System.Collections.Generic;
using Dave6.StatSystem.Effect;

namespace Dave6.StatSystem.Stat
{
    public class SecondaryStat : BaseStat, IDerived
    {
        public List<StatReference> sources { get; private set; } = new();
        public SecondaryStat(StatDefinition definition) : base(definition) { }

        public void SetupSources(List<StatReference> sourceStats)
        {
            sources = sourceStats;

            foreach (var pair in sources)
            {
                pair.sourceStat.onValueChanged += MarkDirty;
            }
            MarkDirty();
        }

        protected override int CalculateBaseInternal()
        {
            int totalWeight = baseValue;

            // total값에 각 sourceStat.finalValue * weight 더하기
            foreach (var pair in sources)
            {
                totalWeight += (int)(pair.sourceStat.finalValue * pair.weight);
            }
            // 최종 base값 반환
            return totalWeight;
        }
    }
}
