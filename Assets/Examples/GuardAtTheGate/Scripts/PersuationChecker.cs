using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using UnityEngine;

namespace Chocolate4.Dialogue.Examples
{
    public class PersuationChecker : MonoBehaviour
    {
        [SerializeField]
        private DialogueMaster dialogueMaster;

        private BasicEventsCollection basicEventsCollection;

        private void OnEnable()
        {
            if (!dialogueMaster.HasInitialized)
            {
                return;
            }

            basicEventsCollection = dialogueMaster.GetCollection<BasicEventsCollection>();
            basicEventsCollection.OnBribeAtTheGate += BasicEventsCollection_OnBribeAtTheGate;
        }

        private void OnDisable()
        {
            basicEventsCollection.OnBribeAtTheGate -= BasicEventsCollection_OnBribeAtTheGate;
        }

        private void Start()
        {
            basicEventsCollection = dialogueMaster.GetCollection<BasicEventsCollection>();
            basicEventsCollection.OnBribeAtTheGate += BasicEventsCollection_OnBribeAtTheGate;
        }

        private void BasicEventsCollection_OnBribeAtTheGate()
        {
            float diceRoll = Random.Range(0f, 1f);

            BasicEventsCollection.SituationName bribeAtTheGateSuccess = 
                BasicEventsCollection.SituationName.BribeAtTheGateSuccess;
            
            BasicEventsCollection.SituationName bribeAtTheGateFailure = 
                BasicEventsCollection.SituationName.BribeAtTheGateFailure;

            if (diceRoll > .5f)
            {
                dialogueMaster.StartSituation(basicEventsCollection.GetSituationName(bribeAtTheGateSuccess));
                return;
            }

            dialogueMaster.StartSituation(basicEventsCollection.GetSituationName(bribeAtTheGateFailure));
        }
    }
}
