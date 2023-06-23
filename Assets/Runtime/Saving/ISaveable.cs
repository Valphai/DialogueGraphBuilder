namespace Chocolate4.Dialogue.Runtime.Saving
{
    public interface ISaveable<T>
    {
        T Save();
        void Load(T saveData);
    }
}
