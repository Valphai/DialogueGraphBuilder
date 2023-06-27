using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    public class BlackboardProvider : IRebuildable<BlackboardSaveData>
    {
        private Dictionary<string, BlackboardRow> propertyRows;
        private BlackboardSection section;

        public List<IDialogueProperty> Properties { get; set; }
        public Blackboard Blackboard { get; private set; }

        public BlackboardProvider(GraphView graphView)
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

            Blackboard.Add(section);
        }

        public BlackboardSaveData Save()
        {
            List<DialoguePropertySaveData> saveData = new List<DialoguePropertySaveData>();
            foreach (IDialogueProperty property in Properties)
            {
                saveData.Add(property.Save());
            }

            return new BlackboardSaveData() { dialoguePropertiesSaveData = saveData } ;
        }

        public void Rebuild(BlackboardSaveData saveData)
        {
            if (saveData.dialoguePropertiesSaveData.IsNullOrEmpty())
            {
                return;
            }

            foreach (DialoguePropertySaveData propertySaveData in saveData.dialoguePropertiesSaveData)
            {
                IDialogueProperty property = propertySaveData.propertyType switch
                {
                    PropertyType.Bool => new BoolDialogueProperty(),
                    PropertyType.Integer => new IntegerDialogueProperty(),
                    _ => throw new NotImplementedException()
                };

                property.Load(propertySaveData);
                AddProperty(property);
                property.UpdateConstantView();

                BlackboardField field = (BlackboardField)propertyRows[property.Id].userData;

                field.text = property.DisplayName;
                UpdateNodesWith(property);
            }
        }

        public void HandlePropertyRemove(IDialogueProperty deletedProperty)
        {
            if (!propertyRows.TryGetValue(deletedProperty.Id, out BlackboardRow row))
            {
                return;
            }

            row.RemoveFromHierarchy();

            Blackboard.graphView.graphElements.ForEach(element => {

                if (!ElementIsDialogueProperty(element, deletedProperty, out IPropertyNode propertyNode))
                {
                    return;
                }

                propertyRows.Remove(deletedProperty.Id);
                propertyNode.UnbindFromProperty();
            });
        }

        internal void UpdatePropertyBinds()
        {
            foreach (IDialogueProperty property in Properties)
            {
                UpdateNodesWith(property);
            }
        }

        private bool ElementIsDialogueProperty(GraphElement element, IDialogueProperty property, out IPropertyNode propertyNode)
        {
            propertyNode = null;
            if (element is not IPropertyNode)
            {
                return false;
            }

            propertyNode = (IPropertyNode)element;
            string propertyGuid = propertyNode.PropertyId;

            if (string.IsNullOrEmpty(propertyGuid))
            {
                return false;
            }

            if (propertyGuid != property.Id)
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
            if (propertyRows.ContainsKey(property.Id))
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
            )
            { userData = property };

            VisualElement expandedAssignValueField = CreateRowExpanded(property);
            BlackboardRow row = new BlackboardRow(field, expandedAssignValueField);

            row.userData = field;

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

            propertyRows[property.Id] = row;

            Properties.Add(property);
            if (create)
            {
                row.expanded = true;
                //m_Graph.owner.RegisterCompleteObjectUndo("Create Property");
                field.OpenTextEditor();
            }
        }

        private VisualElement CreateRowExpanded(IDialogueProperty property)
        {
            VisualElement expandedAssignValueField = new VisualElement()
                .WithFlexGrow()
                .WithHorizontalGrow();

            Label startValueLabel = new Label() { text = "Start value = " };

            IConstantViewControlCreator constantView = property.ToConstantView();
            VisualElement constantViewControl = constantView.CreateControl().WithFlexGrow();

            expandedAssignValueField.Add(startValueLabel);
            expandedAssignValueField.Add(constantViewControl);

            return expandedAssignValueField;
        }
    }
}
