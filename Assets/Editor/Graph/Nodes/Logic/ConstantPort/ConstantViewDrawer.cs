using Chocolate4.Dialogue.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ConstantViewDrawer : ISaveable<List<string>>
	{
        public List<IConstantPortView> ConstantViews { get; set; } = new List<IConstantPortView>();

        public List<string> Save()
        {
            List<string> savedConstantViewValues = new List<string>();
            ConstantViews.ForEach(view => savedConstantViewValues.Add(view.Save()));

            return savedConstantViewValues;
        }

        public void Load(List<string> savedValues)
        {
            if (savedValues.IsNullOrEmpty())
            {
                for (int i = 0; i < ConstantViews.Count; i++)
                {
                    ConstantViews[i].SetDefaultValue();
                }

                return;
            }

            for (int i = 0; i < savedValues.Count; i++)
            {
                ConstantViews[i].Load(savedValues[i]);
            }
        }

        public void HideConstantView(Port referencePort)
        {
            IConstantPortView constantView = ConstantViewUtilities.GetView(referencePort, ConstantViews);
            ConstantPortInput constantPortInput = constantView.GetConstantPortInput();

            constantView.SetDefaultValue();
            constantPortInput.RemoveFromHierarchy();
        }

        public void DisplayConstantView(Port referencePort)
        {
            IConstantPortView constantView = ConstantViewUtilities.GetView(referencePort, ConstantViews);
            ConstantPortInput constantPortInput = constantView.GetConstantPortInput();

            referencePort.Add(constantPortInput);
        }
    }
}