using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using UnityEngine;

namespace Chocolate4.Dialogue.Examples
{
    public class MoneyManipulator : MonoBehaviour
    {
        private const int RoomCost = 300;

        [SerializeField]
        private DialogueMaster dialogueMaster;
        private ShopKeeperCollection shopKeeperCollection;

        private void OnEnable()
        {
            if (!dialogueMaster.HasInitialized)
            {
                return;
            }

            shopKeeperCollection = dialogueMaster.GetCollection<ShopKeeperCollection>();
            shopKeeperCollection.PlayerBoughtRoom += ShopKeeperCollection_PlayerBoughtRoom; 
        }
        
        private void OnDisable()
        {
            shopKeeperCollection.PlayerBoughtRoom -= ShopKeeperCollection_PlayerBoughtRoom;
        }

        private void Start()
        {
            shopKeeperCollection = dialogueMaster.GetCollection<ShopKeeperCollection>();
            shopKeeperCollection.PlayerBoughtRoom += ShopKeeperCollection_PlayerBoughtRoom;
        }

        public void AddMoney()
        {
            shopKeeperCollection.PlayerGold += 999;
        }

        public void GoBroke()
        {
            shopKeeperCollection.PlayerGold = 0;
        }

        private void ShopKeeperCollection_PlayerBoughtRoom()
        {
            shopKeeperCollection.PlayerGold -= RoomCost;
        }
    }
}
