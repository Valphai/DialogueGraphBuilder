using System.Collections.Generic;

namespace Chocolate4.Editor
{
    public abstract class SaveData<T>
    {
        IReadOnlyList<T> data;
    }
}
