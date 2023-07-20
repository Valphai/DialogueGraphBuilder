using Chocolate4.Dialogue.Runtime.Master.Collections;
using UnityEngine;

namespace Chocolate4.Dialogue.Examples
{
    public class ShopKeeperDialogueProgressor : AdvancedDialogueProgressor
    {
        [SerializeField]
        private ShopKeeperCollection.SituationName startSituation;

        public override void StartSituation()
        {
            ShopKeeperCollection collection = dialogueMaster.GetCollection<ShopKeeperCollection>();
            dialogueMaster.StartSituation(collection.GetSituationName(startSituation));
            ProgressInDialogue();
            HideStartConversationButton();
        }
    }
}
