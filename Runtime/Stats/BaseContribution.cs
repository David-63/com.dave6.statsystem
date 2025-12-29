namespace Dave6.StatSystem.Stat
{
    public enum StatValueType
    {
        Flat,               // Base 에 합하는 고정치 (+15 +25)
        Percent,            // 합연산 요소  (+15% +25%)
        finalMultiplier,    // 곱연산 요소  (x1.15 x1.25)
        //Override,           // 값 덮어쓰기
    }
    public class BaseContribution
    {
        public object source;
        public StatValueType valueType { get; private set;}
        public float magnitude { get; private set;}

        public BaseContribution(StatValueType valueType, float magnitude, object source)
        {
            this.valueType = valueType;
            this.magnitude = magnitude;
            this.source = source;
        }
    }
}
