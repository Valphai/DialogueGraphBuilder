using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
	public interface IConstantPortInputUser<T>
	{
        ConstantPortInput CreateConstantPortInput();
        void UpdateConstantViewGenericControl(T value);

        void HideInputField();
        void DisplayInputField();
    }
}