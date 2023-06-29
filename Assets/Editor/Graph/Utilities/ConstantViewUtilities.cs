using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
	public static class ConstantViewUtilities
	{
		public static List<IConstantPortView> CreatePossibleConstantViews(Port referencePort)
		{
			List<IConstantPortView> result = new List<IConstantPortView>();

			List<Type> possibleConstantViews = TypeExtensions.GetTypes<IConstantPortView>().ToList();
			foreach (Type type in possibleConstantViews)
			{
                result.Add(CreateConstantView(type, referencePort));
            }

			return result;
		}

		public static IConstantPortView CreateConstantView(Type type, Port referencePort)
		{
            IConstantPortView instance = (IConstantPortView)Activator.CreateInstance(type);
			instance.BindToPort(referencePort);
			return instance;
		}

        public static IConstantPortView GetView(Port referencePort, List<IConstantPortView> constantViews)
        {
            return constantViews.Find(view => view.ReferencePort == referencePort && referencePort.portType == view.PortType);
        }
    }
}