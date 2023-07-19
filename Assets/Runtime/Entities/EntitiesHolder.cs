using Chocolate4.Dialogue.Runtime.Saving;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Entities
{
    public class EntitiesHolder : ScriptableObject
	{
        public const string DataBase = "Entities Database";

        public string associatedAssetName;

        [SerializeField]
        private List<DialogueEntity> dataBase = new List<DialogueEntity>();

        public IReadOnlyCollection<DialogueEntity> DialogueEntities => dataBase;

        [ContextMenu("Reload")]
		public void Reload()
		{
            dataBase.Clear();

            dataBase.AddRange(
                EntitiesUtilities.GetAllEntities(this)
            );
        }
	}
}