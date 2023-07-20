using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Master.Collections
{
    public class SimpleProgressionCollection : IDialogueMasterCollection
    {
        public Type CollectionType => typeof(SimpleProgressionCollection);

        #region Situation Names
        public enum SituationName
        {
            PrincessApproachesKing,
        }

        public const string PrincessApproachesKing = "Princess approaches king";

        private Dictionary<SituationName, string> situations = new Dictionary<SituationName, string>
        {
            { SituationName.PrincessApproachesKing, PrincessApproachesKing },
        };

        public string GetSituationName(SituationName situationName) => situations[situationName];

        #endregion

        #region Variables
        #endregion

        #region Events
        #endregion
    }
}
