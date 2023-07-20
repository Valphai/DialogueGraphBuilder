using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Master.Collections
{
    public class BasicEventsCollection : IDialogueMasterCollection
    {
        public Type CollectionType => typeof(BasicEventsCollection);

        #region Situation Names
        public enum SituationName
        {
            MetTheGuardAtTheGateFirstTime,
            BribeAtTheGateSuccess,
            BribeAtTheGateFailure,
        }

        public const string MetTheGuardAtTheGateFirstTime = "Met the guard at the gate first time";
        public const string BribeAtTheGateSuccess = "Bribe at the gate success";
        public const string BribeAtTheGateFailure = "Bribe at the gate failure";

        private Dictionary<SituationName, string> situations = new Dictionary<SituationName, string>
        {
            { SituationName.MetTheGuardAtTheGateFirstTime, MetTheGuardAtTheGateFirstTime },
            { SituationName.BribeAtTheGateSuccess, BribeAtTheGateSuccess },
            { SituationName.BribeAtTheGateFailure, BribeAtTheGateFailure },
        };

        public string GetSituationName(SituationName situationName) => situations[situationName];

        #endregion

        #region Variables
        #endregion

        #region Events
        public event Action OnBribeAtTheGate;

        private void InvokeOnBribeAtTheGate() => OnBribeAtTheGate?.Invoke();
        #endregion
    }
}
