using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [CreateAssetMenu(fileName = DataBase, menuName = FilePathConstants.Chocolate4 + "/" + nameof(EntitiesHolder))]
    public class EntitiesHolder : ScriptableObject
	{
        public const string DataBase = "Entities Database";

        [SerializeField]
        private List<DialogueEntity> dataBase;

        public IReadOnlyCollection<DialogueEntity> DialogueEntities => dataBase;

        [ContextMenu("Reload")]
		public void Reload()
		{
            dataBase.Clear();

            string[] filePaths = Directory.GetFiles(
                FilePathConstants.GetPathRelativeTo(FilePathConstants.Assets, FilePathConstants.dialogueEntitiesPath), "*.asset"
            );

            filePaths = filePaths.Where(path => !path.Contains(name)).ToArray();

            foreach (string path in filePaths)
            {
                dataBase.Add(
                    AssetDatabase.LoadAssetAtPath<DialogueEntity>(path)
                );
            }
        }
	}
}