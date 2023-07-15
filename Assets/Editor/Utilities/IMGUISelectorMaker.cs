using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Utilities
{
    public class IMGUISelectorMaker
    {
        public IMGUIContainer MakeIMGUISelector<T>(Action<T> onSelected) where T : UnityEngine.Object
        {
            IMGUIContainer imguiContainer = new IMGUIContainer(() => {

                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                if (
                    Event.current.commandName.Equals("ObjectSelectorUpdated")
                    && EditorGUIUtility.GetObjectPickerControlID() == controlID
                )
                {
                    onSelected?.Invoke((T)EditorGUIUtility.GetObjectPickerObject());
                }

                if (!GUILayout.Button("Select"))
                {
                    return;
                }

                EditorGUIUtility.ShowObjectPicker<T>(null, false, string.Empty, controlID);
            });
            imguiContainer.style.alignSelf = Align.FlexEnd;
            imguiContainer.style.top = 82f;

            return imguiContainer;
        }
    }
}