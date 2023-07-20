using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using Chocolate4.Dialogue.Runtime.Saving;

namespace Chocolate4.Dialogue.Examples
{
    public class ShopKeeperNameProcessor : IEntityProcessor
    {
        private ShopKeeperCollection shopKeeperCollection;

        public DialogueEntity Process(DialogueMaster master, DialogueEntity speaker)
        {
            shopKeeperCollection ??= master.GetCollection<ShopKeeperCollection>();

            speaker.entityName = shopKeeperCollection.SimonIntroducedHimself 
                ? speaker.extraText[0] : speaker.entityName;

            return speaker;
        }
    }
}
