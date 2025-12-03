namespace SaveSystem
{
    public interface ISaveable
    {
        object data { get; }
        void Load(object data);
    }
}