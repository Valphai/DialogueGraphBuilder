using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Runtime.Utilities.Parsing;
using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger
{
	public static class DangerDetector
    {
        public static void SanitizeExpression(BaseNode dangerCauser, string newText)
        {
            if (dangerCauser is not ITextHolder textHolder)
            {
                return;
            }

            textHolder.Text = newText;
            if (newText.Contains(ParseConstants.Equal))
            {
                int equalIndex = newText.IndexOf(ParseConstants.Equal);
                if (equalIndex - 1 < 0 || equalIndex + 1 > newText.Length)
                {
                    DangerLogger.MarkNodeDangerous($"Invalid expression detected in {dangerCauser}. Equal sign is in a wrong position.", dangerCauser,
                        () => {

                            List<int> foundIndexes = new List<int>();
                            for (int i = textHolder.Text.IndexOf(ParseConstants.Equal); i > -1; i = textHolder.Text.IndexOf(ParseConstants.Equal, i + 1))
                            {
                                foundIndexes.Add(i);
                            }

                            int equalIndex = textHolder.Text.IndexOf(ParseConstants.Equal);
                            return !textHolder.Text.Contains(ParseConstants.Equal) 
                                || foundIndexes.All(index => equalIndex - 1 > 0 && equalIndex + 1 < textHolder.Text.Length);
                        }
                    );
                }

                int semicolonCount = newText.Count(c => c == ';');
                int semicolonExpressions = newText.Split(';').Length;
                if (semicolonCount != semicolonExpressions)
                {
                    DangerLogger.MarkNodeDangerous($"Invalid expression detected in {dangerCauser}. All expressions must end with \";\".", dangerCauser,
                        () => {
                            int semicolonCount = newText.Count(c => c == ';');
                            int semicolonExpressions = newText.Split(';').Length;

                            return semicolonCount == semicolonExpressions;
                        }
                    );
                }
            }
        }
    }
}