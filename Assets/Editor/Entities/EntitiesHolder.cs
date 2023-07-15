using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Entities.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Entities
{
    [CreateAssetMenu(fileName = DataBase, menuName = FilePathConstants.Chocolate4 + "/" + nameof(EntitiesHolder))]
    public class EntitiesHolder : ScriptableObject
	{
        public const string DataBase = "Entities Database";

        [SerializeField]
        private List<DialogueEntity> dataBase = new List<DialogueEntity>();

        public IReadOnlyCollection<DialogueEntity> DialogueEntities => dataBase;

        [ContextMenu("Reload")]
		public void Reload()
		{
            dataBase.Clear();

            dataBase.AddRange(
                EntitiesUtilities.GetAllEntities(this, out string _)
            );
        }
	}
}