using Chocolate4.Dialogue.Runtime.Master.Collections;
using UnityEngine;

namespace Chocolate4.Dialogue.Examples
{
    public class GateGuardDialogueProgressor : AdvancedDialogueProgressor
    {
        [SerializeField]
        private BasicEventsCollection.SituationName startSituation;

        public override void StartSituation()
        {
            BasicEventsCollection collection = dialogueMaster.GetCollection<BasicEventsCollection>();
            dialogueMaster.StartSituation(collection.GetSituationName(startSituation));
            ProgressInDialogue();
            HideStartConversationButton();
        }
    }
}
