using B83.LogicExpressionParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chocolate4.Runtime.Utilities.Parsing
{
	public class ParseAdapter
	{
        private Parser parser;

        public ParseAdapter()
        {
            parser = new Parser();

            parser.ExpressionContext["wood"].Set(15);
            parser.ExpressionContext["someVar"].Set(30);
            parser.ExpressionContext["stone"].Set(78);
        }

        public bool IsAssignment(string expression, out string oper)
        {
            oper = string.Empty;

            if (expression.Contains(ParseConstants.Equal))
            {
                int equalIndex = expression.IndexOf(ParseConstants.Equal);
                if (!expression[equalIndex - 1].Equals(ParseConstants.Equal) && !expression[equalIndex + 1].Equals(ParseConstants.Equal))
                {
                    oper = ParseConstants.Equal;
                    return true;
                }
            }
            else if (expression.Contains(ParseConstants.PlusEqual))
            {
                oper = ParseConstants.PlusEqual;
                return true;
            }
            else if (expression.Contains(ParseConstants.MinusEqual))
            {
                oper = ParseConstants.MinusEqual;
                return true;
            }
            else if (expression.Contains(ParseConstants.TimesEqual))
            {
                oper = ParseConstants.TimesEqual;
                return true;
            }
            else if (expression.Contains(ParseConstants.DivEqual))
            {
                oper = ParseConstants.DivEqual;
                return true;
            }

            return false;
        }

        public void EvaluateSetExpressions(string expressions)
        {
            string[] expressionsToParse = expressions.Split(";");

            for (int i = 0; i < expressionsToParse.Length; i++)
            {
                string expression = expressionsToParse[i];

                if (!IsAssignment(expression, out string oper))
                {
                    Debug.LogWarning($"Detected invalid set expression {expression}! It was ignored.");
                    continue;
                }

                EvaluateSetExpression(expression, oper);
            }
        }

        public void EvaluateSetExpression(string expression, string oper)
        {
            string expressionNoSpace = new string(expression.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            string[] split = expressionNoSpace.Split(oper); // or different supported operands

            NumberExpression num = parser.ParseNumber(split.Last());

            double val = num.GetNumber();

            // update VariableCollection with reflection

            ExpressionVariable expressionVariable = parser.ExpressionContext[split.First()];
            expressionVariable.Set(() => val);

            Debug.Log(expressionVariable.GetNumber());
        }

        public bool EvaluateConditions(string expressions)
        {
            string[] expressionsToParse = expressions.Split(";");

            List<bool> conditions = new List<bool>();
            for (int i = 0; i < expressionsToParse.Length; i++)
            {
                string expression = expressionsToParse[i];
                conditions.Add(EvaluateCondition(expression));
            }

            return conditions.All(condition => condition == true);
        }

        private bool EvaluateCondition(string expression)
        {
            LogicExpression exp = parser.Parse(expression);
            return exp.GetResult();
        }
    }
}