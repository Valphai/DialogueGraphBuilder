using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Common.Edit
{
    [CustomPropertyDrawer(typeof(InterfaceListAttribute))]
    public class InterfaceListDrawer : PropertyDrawer
    {
        private int selectedIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            Type attributeMarkedType = fieldInfo.FieldType.GetGenericArguments().First();

            TypeCache.TypeCollection implementingTypes = TypeCache.GetTypesDerivedFrom(attributeMarkedType);
            
            selectedIndex = EditorGUI.Popup(position, selectedIndex, implementingTypes.Select(type => type.Name).ToArray());

            Type implementingType = implementingTypes[selectedIndex];
            object managedReferenceValue = property.managedReferenceValue;

            if (property.managedReferenceValue != null)
            {
                if (managedReferenceValue.GetType() == implementingType)
                {
                    return;
                } 
            }

            var selectedTypeInstance = Activator.CreateInstance(implementingType);
            property.managedReferenceValue = selectedTypeInstance;
            
            property.serializedObject.ApplyModifiedProperties();
        }
    } 
}
