using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Master.Collections
{
    public class TestCasesDialogueEditorCollection : IDialogueMasterCollection
    {
        public Type CollectionType => typeof(TestCasesDialogueEditorCollection);

        #region Situation Names
        public enum SituationName
        {
            Test,
            Test1,
            Test2,
            Test3,
            Test4,
            Test5,
            Test6,
            Test7,
        }

        public const string Test = "Test";
        public const string Test1 = "Test (1)";
        public const string Test2 = "Test (2)";
        public const string Test3 = "Test (3)";
        public const string Test4 = "Test (4)";
        public const string Test5 = "Test (5)";
        public const string Test6 = "Test (6)";
        public const string Test7 = "Test (7)";

        private Dictionary<SituationName, string> situations = new Dictionary<SituationName, string>
        {
            { SituationName.Test, Test },
            { SituationName.Test1, Test1 },
            { SituationName.Test2, Test2 },
            { SituationName.Test3, Test3 },
            { SituationName.Test4, Test4 },
            { SituationName.Test5, Test5 },
            { SituationName.Test6, Test6 },
            { SituationName.Test7, Test7 },
        };

        public string GetSituationName(SituationName situationName) => situations[situationName];

        #endregion

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
