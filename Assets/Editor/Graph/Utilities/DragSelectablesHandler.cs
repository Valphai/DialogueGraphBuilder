using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public class DragSelectablesHandler
    {
        [field: SerializeField]
        public IDialogueProperty SelectedProperty { get; private set; }

        public void RegisterSelectedProperty(IDialogueProperty selectedProperty)
        {
            SelectedProperty = selectedProperty;
        }

        public void UnregisterSelectedProperty()
        {
            SelectedProperty = null;
        }
    } 
}