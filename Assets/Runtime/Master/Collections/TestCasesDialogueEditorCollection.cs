using System;

namespace Chocolate4.Dialogue.Runtime.Master.Collections
{
    public class TestCasesDialogueEditorCollection : IDialogueMasterCollection
    {
        public Type CollectionType => typeof(TestCasesDialogueEditorCollection);
        #region Variables
        public int MyInt { get; set; } = 4;

        public bool MyBool { get; set; } = true;

        #endregion

        #region Events
        public event Action MyEvent;

        private void InvokeMyEvent() => MyEvent?.Invoke();

        #endregion
    }
}
