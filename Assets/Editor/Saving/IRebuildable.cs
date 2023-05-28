namespace Chocolate4.Editor
{
    internal interface IRebuildable<T>
    {
        SaveData<T> Save();
        void Rebuild(SaveData<T> saveData);
    }
}
