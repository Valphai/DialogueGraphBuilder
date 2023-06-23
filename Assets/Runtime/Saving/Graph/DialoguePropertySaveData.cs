using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class DialoguePropertySaveData
    {
        public string value;
        public string id;
        public PropertyType propertyType;
        public string displayName;
    }
}
