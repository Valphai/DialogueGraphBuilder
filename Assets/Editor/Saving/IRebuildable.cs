namespace Chocolate4.Editor
{
    internal interface IRebuildable<T>
    {
        T Save();
        void Rebuild(T saveData);
    }
}
