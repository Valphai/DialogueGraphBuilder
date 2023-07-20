using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Utilities.Parsing;
using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger
{
    public static class DangerDetector
    {
        internal static void SanitizeExpression(BaseNode dangerCauser, string newText)
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
                    if (dangerCauser.IsMarkedDangerous)
                    {
                        return;
                    }

                    DangerLogger.ErrorDanger($"Invalid expression detected in {dangerCauser}. Equal sign is in a wrong position.", dangerCauser);
                    DangerLogger.MarkNodeDangerous(dangerCauser,
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

                    return;
                }
            }

            if (!IsExpressionValid(newText))
            {
                if (dangerCauser.IsMarkedDangerous)
                {
                    return;
                }

                DangerLogger.ErrorDanger($"Invalid expression detected in {dangerCauser}. All expressions must end with \";\".", dangerCauser);
                DangerLogger.MarkNodeDangerous(dangerCauser,
                    () => IsExpressionValid(newText)
                );

                return;
            }

            DangerLogger.UnmarkNodeDangerous(dangerCauser);
        }

        internal static void SanitizeTransferNodes<T>(T sanitizedNode, DialogueGraphView graphView) where T : SituationTransferNode
        {
            List<T> transferNodes = new List<T>();

            graphView.PerformOnAllGraphElementsOfType<T>(dangerNode => transferNodes.Add(dangerNode));

            if (!transferNodes.Contains(sanitizedNode))
            {
                transferNodes.Add(sanitizedNode);
            }

            List<IGrouping<string, T>> groups = transferNodes.GroupBy(node => node.NextSituationId).ToList();
            foreach (var group in groups)
            {
                int groupCount = group.Count();
                if (groupCount <= 1)
                {
                    T dangerousNode = group.First();
                    DangerLogger.UnmarkNodeDangerous(dangerousNode);
                    continue;
                }

                foreach (T dangerNode in group)
                {
                    if (dangerNode.IsMarkedDangerous)
                    {
                        continue;
                    }

                    if (dangerNode == sanitizedNode)
                    {
                        DangerLogger.ErrorDanger(
                            $"{sanitizedNode} has the same situation selected as {groupCount - 1} other nodes, this is not supported! Each node must have different situation selected.",
                            sanitizedNode
                        );
                    }

                    DangerLogger.MarkNodeDangerous(dangerNode,
                        () => transferNodes.All(node => !dangerNode.NextSituationId.Equals(node.NextSituationId))
                    );
                }
            }
        }

        private static bool IsExpressionValid(string expression)
        {
            int semicolonCount = expression.Count(c => c == ';');
            expression = expression.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty);
            string[] expressionsToParse = expression.Split(";").Where(str => !string.IsNullOrEmpty(str)).ToArray();
            int semicolonExpressions = expressionsToParse.Length;

            return semicolonCount == semicolonExpressions;
        }
    }
}