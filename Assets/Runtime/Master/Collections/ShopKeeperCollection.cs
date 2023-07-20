using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Master.Collections
{
    public class ShopKeeperCollection : IDialogueMasterCollection
    {
        public Type CollectionType => typeof(ShopKeeperCollection);

        #region Situation Names
        public enum SituationName
        {
            JustEnteredTheShop,
            BackToTheShop,
            LookingForWork,
            MysticForestQuestIntro,
        }

        public const string JustEnteredTheShop = "Just entered the shop";
        public const string BackToTheShop = "Back to the shop";
        public const string LookingForWork = "Looking for work";
        public const string MysticForestQuestIntro = "Mystic forest quest intro";

        private Dictionary<SituationName, string> situations = new Dictionary<SituationName, string>
        {
            { SituationName.JustEnteredTheShop, JustEnteredTheShop },
            { SituationName.BackToTheShop, BackToTheShop },
            { SituationName.LookingForWork, LookingForWork },
            { SituationName.MysticForestQuestIntro, MysticForestQuestIntro },
        };

        public string GetSituationName(SituationName situationName) => situations[situationName];

        #endregion

        #region Variables
        public bool FirstTimeInTheShop { get; set; } = true;
        public bool SimonIntroducedHimself { get; set; } = false;
        public int PlayerGold { get; set; } = 0;
        public bool IntroducedMysticForestQuest { get; set; } = false;
        #endregion

        #region Events
        public event Action OpenedShop;
        public event Action PlayerBoughtRoom;

        private void InvokeOpenedShop() => OpenedShop?.Invoke();

        private void InvokePlayerBoughtRoom() => PlayerBoughtRoom?.Invoke();
        #endregion
    }
}
