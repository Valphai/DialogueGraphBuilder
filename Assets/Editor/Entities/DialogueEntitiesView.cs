using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Entities.Sidebar;
using Chocolate4.Edit.Entities.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
            cachedDialogueEntities = entitiesData.cachedEntities;
            CreateListView();
            ListView.Rebuild();
        }

        public EntitiesData Save()
        {
            return new EntitiesData() { cachedEntities = cachedDialogueEntities };
        }

        internal DialogueEntity AddEntity()
        {
            DialogueEntity entity = ScriptableObject.CreateInstance<DialogueEntity>();
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
            entityView.Display(entity);
        }

        private void BindItem(VisualElement item, int index)
        {
            ListViewItem listViewItem = item as ListViewItem;
            DialogueEntity entity = cachedDialogueEntities[index];

            Texture2D icon = EntitiesUtilities.GetEntityImage(entity);
            listViewItem.Initialize(entity.EntityName, icon);
        }

    }
}
