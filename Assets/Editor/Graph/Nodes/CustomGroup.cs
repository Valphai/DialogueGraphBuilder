using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Graph.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
	public class CustomGroup : Group, IHaveId, ISaveable<GroupSaveData>
    {
		private string id;

        public string Id => id;

        public CustomGroup() : base()
        {
            id = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return $"{title} : {id}";
        }

        public GroupSaveData Save()
        {
            return new GroupSaveData()
            {
                id = id,
                displayName = title,
                position = this.GetPositionRaw(),
            };
        }

        public void Load(GroupSaveData saveData)
        {
            id = saveData.id;
            title = saveData.displayName;
            SetPosition(new Rect(saveData.position, Vector2.zero));
        }

        internal void AddToGroup(BaseNode node)
        {
            AddElement(node);
            node.GroupId = Id;
        }
    }
}