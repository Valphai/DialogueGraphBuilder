using System;

namespace Chocolate4.Dialogue.Runtime
{
    public class DialogueMasterCollection
    {
        #region Variables
        public int MyInt { get; set; } = 4;
        public bool MyBool { get; set; } = true;
        #endregion

        #region Events
        public event Action MyEvent;
        #endregion
    }
}
