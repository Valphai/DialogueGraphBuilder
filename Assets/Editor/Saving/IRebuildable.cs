namespace Chocolate4.Dialogue.Edit.Saving
{
    public interface IRebuildable<T>
    {
        T Save();
        void Rebuild(T saveData);
    }
}
