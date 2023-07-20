using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger;
using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Dialogue.Edit.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    PropertyType.Event => new EventDialogueProperty(),
                    _ => throw new NotImplementedException()
                };

                property.Load(propertySaveData);
                AddProperty(property);
                if (property is IExpandableDialogueProperty expandableProperty)
                {
                    expandableProperty.UpdateConstantView();
                }

                BlackboardField field = (BlackboardField)propertyRows[property.Id].userData;

                field.text = property.DisplayName;
                UpdateNodesWith(property);
            }
        }

        internal void HandlePropertyRemove(IDialogueProperty deletedProperty)
        {
            if (!propertyRows.TryGetValue(deletedProperty.Id, out BlackboardRow row))
            {
                return;
            }

            row.RemoveFromHierarchy();
            Properties.Remove(deletedProperty);

            

            Blackboard.graphView.graphElements.ForEach(element => {

                if (!ElementIsDialogueProperty(element, deletedProperty, out IPropertyNode propertyNode))
                {
                    return;
                }

                propertyRows.Remove(deletedProperty.Id);
                propertyNode.UnbindFromProperty();

                BaseNode dangerCauser = (BaseNode)propertyNode;
                DangerLogger.ErrorDanger(
                    "Deleted property created an empty node! Remove the node or convert it back to property.", dangerCauser
                );

                DangerLogger.MarkNodeDangerous(
                    dangerCauser, () => {
                        if (propertyNode.IsBoundToProperty)
                        {
                            DangerLogger.UnmarkNodeDangerous(dangerCauser);
                            return true;
                        }
                        return false;
                    }
                );
            });
        }

        internal void UpdatePropertyBinds()
        {
            foreach (IDialogueProperty property in Properties)
            {
                UpdateNodesWith(property);
            }
        }

        internal void AddProperty(IDialogueProperty property, bool create = false, int index = -1)
        {
            if (propertyRows.ContainsKey(property.Id))
            {
                return;
            }

            if (create)
            {
                property.DisplayName = GetSanitizedPropertyName(property.DisplayName);
            }

            BlackboardField field;

            if (property is IDraggableProperty draggableProperty)
            {
                field = new BlackboardDraggableField(
                    (DialogueGraphView)Blackboard.graphView,
                    property.DisplayName,
                    property.PropertyType.ToString()
                )
                { userData = draggableProperty };
            }
            else
            {
                field = new BlackboardField(
                    null,
                    property.DisplayName,
                    property.PropertyType.ToString()
                )
                { userData = property };
            }

            BlackboardRow row;
            if (property is IExpandableDialogueProperty expandableProperty)
            {
                VisualElement expandedAssignValueField = CreateRowExpanded(expandableProperty);
                row = new BlackboardRow(field, expandedAssignValueField);
            }
            else
            {
                row = new BlackboardRow(field, null);
            }

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

        private string GetSanitizedPropertyName(string propertyName)
        {
            string[] existingNames = Properties.Select(property => property.DisplayName).ToArray();
            return ObjectNames.GetUniqueName(existingNames, propertyName);
        }

        private void AddItemRequested(Blackboard blackboard)
        {
            var gm = new GenericMenu();
            gm.AddItem(new GUIContent("Integer"), false, () => AddProperty(new IntegerDialogueProperty(), true));
            gm.AddItem(new GUIContent("Bool"), false, () => AddProperty(new BoolDialogueProperty(), true));
            gm.AddItem(new GUIContent("Event"), false, () => AddProperty(new EventDialogueProperty(), true));
            gm.ShowAsContext();
        }

        private void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            BlackboardField field = (BlackboardField)visualElement;
            IDialogueProperty property = (IDialogueProperty)field.userData;

            newText = newText.Sanitize();
            if (!string.IsNullOrEmpty(newText) && !newText.Equals(property.DisplayName))
            {
                //m_Graph.owner.RegisterCompleteObjectUndo("Edit Property Name");
                newText = GetSanitizedPropertyName(newText);

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

        private bool ElementIsDialogueProperty(GraphElement element, IDialogueProperty property, out IPropertyNode propertyNode)
        {
            propertyNode = null;
            if (element is not IPropertyNode)
            {
                return false;
            }

            propertyNode = (IPropertyNode)element;
            string propertyId = propertyNode.PropertyId;

            if (string.IsNullOrEmpty(propertyId))
            {
                return false;
            }

            if (propertyId != property.Id)
            {
                return false;
            }

            return true;
        }

        private VisualElement CreateRowExpanded(IExpandableDialogueProperty property)
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
