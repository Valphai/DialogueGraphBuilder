namespace Chocolate4.Dialogue.Edit.Saving
{
    internal interface IRebuildable<T>
    {
        T Save();
        void Rebuild(T saveData);
    }
}
