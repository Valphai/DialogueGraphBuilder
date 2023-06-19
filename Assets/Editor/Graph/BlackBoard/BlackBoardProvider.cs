using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Edit.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    public class BlackBoardProvider
    {
        private Dictionary<string, BlackboardRow> propertyRows;
        private BlackboardSection section;

        public List<IDialogueProperty> Properties { get; set; }

        public Blackboard Blackboard { get; private set; }

        public BlackBoardProvider(GraphView graphView)
        {
            propertyRows = new Dictionary<string, BlackboardRow>();

            Blackboard = new Blackboard(graphView)
            {
                scrollable = true,
                title = "Globals",
                editTextRequested = EditTextRequested,
                addItemRequested = AddItemRequested,
                moveItemRequested = MoveItemRequested,
            };
            
            section = new BlackboardSection { headerVisible = false };

            Properties = new List<IDialogueProperty>();
            foreach (var property in Properties)
            {
                AddProperty(property);
            } 

            Blackboard.Add(section);
        }

        public void HandlePropertyRemove(IDialogueProperty deletedProperty)
        {
            if (!propertyRows.TryGetValue(deletedProperty.Guid, out BlackboardRow row))
            {
                return;
            }

            row.RemoveFromHierarchy();

            Blackboard.graphView.graphElements.ForEach(element => {

                if (!ElementIsDialogueProperty(element, deletedProperty, out IPropertyNode propertyNode))
                {
                    return;
                }

                propertyRows.Remove(deletedProperty.Guid);
                propertyNode.UnbindFromProperty();
            });
        }

        private bool ElementIsDialogueProperty(GraphElement element, IDialogueProperty property, out IPropertyNode propertyNode)
        {
            propertyNode = null;
            if (element is not IPropertyNode)
            {
                return false;
            }

            propertyNode = (IPropertyNode)element;
            string propertyGuid = propertyNode.PropertyGuid;

            if (string.IsNullOrEmpty(propertyGuid))
            {
                return false;
            }

            if (propertyGuid != property.Guid)
            {
                return false;
            }

            return true;
        }

        private void MoveItemRequested(Blackboard arg1, int arg2, VisualElement arg3)
        {
            throw new NotImplementedException();
        }

        private void AddItemRequested(Blackboard blackboard)
        {
            var gm = new GenericMenu();
            gm.AddItem(new GUIContent("Integer"), false, () => AddProperty(new IntegerDialogueProperty(), true));
            gm.AddItem(new GUIContent("Bool"), false, () => AddProperty(new BoolDialogueProperty(), true));
            gm.ShowAsContext();
        }

        private void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            BlackboardField field = (BlackboardField)visualElement;
            IDialogueProperty property = (IDialogueProperty)field.userData;

            if (!string.IsNullOrEmpty(newText) && newText != property.DisplayName)
            {
                //m_Graph.owner.RegisterCompleteObjectUndo("Edit Property Name");
                //newText = m_Graph.SanitizePropertyName(newText, property.guid);
                property.DisplayName = newText;
                field.text = newText;
                UpdateNodesWith(property);
                //DirtyNodes();
            }
        }

        private void UpdateNodesWith(IDialogueProperty property)
        {
            Blackboard.graphView.graphElements.ForEach(element => {

                if (!ElementIsDialogueProperty(element, property, out IPropertyNode propertyNode))
                {
                    return;
                }

                propertyNode.BindToProperty(property);
            });
        }

        public void AddProperty(IDialogueProperty property, bool create = false, int index = -1)
        {
            if (propertyRows.ContainsKey(property.Guid))
            {
                return;
            }

            if (create)
            {
                //property.DisplayName = m_Graph.SanitizePropertyName(property.DisplayName);
            }

            BlackboardDraggableField field = new BlackboardDraggableField(
                (DialogueGraphView)Blackboard.graphView, 
                property.DisplayName, 
                property.PropertyType.ToString()
            ) { userData = property };

            BlackboardRow row = new BlackboardRow(field, null);

            row.userData = property;

            if (index < 0)
            {
                index = propertyRows.Count;
            }

            if (index == propertyRows.Count)
            {
                section.Add(row);
            }
            else
            {
                section.Insert(index, row);
            }

            propertyRows[property.Guid] = row;

            if (create)
            {
                row.expanded = true;
                //m_Graph.owner.RegisterCompleteObjectUndo("Create Property");
                Properties.Append(property);
                field.OpenTextEditor();
            }
        }
    }
}
