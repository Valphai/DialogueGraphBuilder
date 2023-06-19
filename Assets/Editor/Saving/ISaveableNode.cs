using Chocolate4.Dialogue.Runtime.Saving;

namespace Chocolate4.Dialogue.Edit.Saving
{
    internal interface ISaveableNode
    {
        IDataHolder Save();
        void Load(IDataHolder saveData);
    }
}
