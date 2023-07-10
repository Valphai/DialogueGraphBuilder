using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Entities.Sidebar;
using Chocolate4.Edit.Entities.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Edit.Entities
{
    public class DialogueEntitiesView : VisualElement, IRebuildable<EntitiesData>
    {
        private List<DialogueEntity> cachedDialogueEntities;
        private EntityView entityView;

        public ListView ListView { get; private set; }

        internal void Initialize()
        {
            ResolveDependencies();
        }

        public void Rebuild(EntitiesData entitiesData)
        {
            cachedDialogueEntities = entitiesData.cachedEntities.ToList();
            
            CreateListView();

            if (!cachedDialogueEntities.IsNullOrEmpty())
            {
                ListView.SetSelection(0);
            }

            ListView.Rebuild();
        }

        public EntitiesData Save()
        {
            return new EntitiesData() { cachedEntities = cachedDialogueEntities };
        }

        internal DialogueEntity AddEntity()
        {
            DialogueEntity entity = ScriptableObject.CreateInstance<DialogueEntity>();

            string[] existingNames = cachedDialogueEntities.Select(entity => entity.entityName).ToArray();
            string name = EntitiesUtilities.GetEntityName(entity, existingNames);

            entity.entityName = name;

            cachedDialogueEntities.Add(entity);
            ListView.Rebuild();
            return entity;
        }

        private void ResolveDependencies()
        {
            cachedDialogueEntities = new List<DialogueEntity>();

            entityView = new EntityView();
            contentContainer.Add(entityView);
            entityView.StretchToParentSize();
        }

        private void CreateListView()
        {
            ListView = new ListView() { reorderable = true, fixedItemHeight = EntitiesConstants.ListViewItemHeight };
            ListView.selectedIndex = 0;
            
            ListView.itemsSource = cachedDialogueEntities;
            ListView.makeItem = MakeListViewItem;
            ListView.bindItem = (item, index) => BindItem(item, index);
            ListView.selectionChanged += ListView_selectionChanged;
        }

        private VisualElement MakeListViewItem()
        {
            return new ListViewItem();
        }

        private void ListView_selectionChanged(IEnumerable<object> selectedItems)
        {
            DialogueEntity entity = (DialogueEntity)selectedItems.First();
            
            ListView.Rebuild();
            entityView.Display(entity, (updatedEntity) => ListView.RefreshItem(ListView.selectedIndex));
        }

        private void BindItem(VisualElement item, int index)
        {
            ListViewItem listViewItem = item as ListViewItem;
            DialogueEntity entity = cachedDialogueEntities[index];

            listViewItem.Initialize(entity);
        }
    }
}
