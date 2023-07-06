using B83.LogicExpressionParser;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Chocolate4.Runtime.Utilities.Parsing
{
	public class ParseAdapter
	{
        private Parser parser;
        private IDialogueMasterCollection collection;

        private Type CollectionType => collection.CollectionType;

        public ParseAdapter(IDialogueMasterCollection collection)
        {
            parser = new Parser();

            this.collection = collection;
            SetVariables();
        }

        public void EvaluateSetExpressions(string expressions)
        {
            string[] expressionsToParse = SanitizeExpression(expressions);

            for (int i = 0; i < expressionsToParse.Length; i++)
            {
                string expression = expressionsToParse[i];

                if (!IsAssignment(expression, out string oper, out IParseOperator parseOperator))
                {
                    Debug.LogWarning($"Detected invalid set expression {expression}! It was ignored.");
                    continue;
                }

                EvaluateSetExpression(expression, oper, parseOperator);
            }
        }

        public void EvaluateSetExpression(string expression, string oper, IParseOperator parseOperator)
        {
            string expressionNoSpace = new string(expression.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            string[] split = expressionNoSpace.Split(oper);

            SetCollectionVariable(split.First(), split.Last(), parseOperator);
        }

        public bool EvaluateConditions(string expressions)
        {
            string[] expressionsToParse = SanitizeExpression(expressions);

            List<bool> conditions = new List<bool>();
            for (int i = 0; i < expressionsToParse.Length; i++)
            {
                string expression = expressionsToParse[i];
                conditions.Add(EvaluateCondition(expression));
            }

            return conditions.All(condition => condition == true);
        }

        private string[] SanitizeExpression(string expressions)
        {
            expressions.Replace(Environment.NewLine, string.Empty);
            string[] expressionsToParse = expressions.Split(";").Where(str => !string.IsNullOrEmpty(str)).ToArray();
            return expressionsToParse;
        }

        private bool EvaluateCondition(string expression)
        {
            LogicExpression exp = parser.Parse(expression);
            return exp.GetResult();
        }

        private void SetVariables()
        {
            MemberInfo[] members = CollectionType.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];
                if (memberInfo.MemberType is not MemberTypes.Property)
                {
                    continue;
                }

                PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                SetParserVariable(propertyInfo);
            }
        }

        private void SetParserVariable(PropertyInfo propertyInfo)
        {
            string name = propertyInfo.Name;
            if (propertyInfo.PropertyType == typeof(int))
            {
                parser.ExpressionContext[name].Set(
                    () => (int)Convert.ChangeType(CollectionType.GetProperty(name).GetValue(collection), typeof(int))
                );
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                parser.ExpressionContext[name].Set(
                    () => (bool)Convert.ChangeType(CollectionType.GetProperty(name).GetValue(collection), typeof(bool))
                );
            }
        }

        private void SetCollectionVariable(string propertyName, string expression, IParseOperator parseOperator)
        {
            PropertyInfo propertyInfo = CollectionType.GetProperty(propertyName);
            if (propertyInfo.PropertyType == typeof(int))
            {
                ParseInt(propertyName, expression, parseOperator, propertyInfo);
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                ParseBool(expression, propertyInfo);
            }
        }

        private void ParseBool(string expression, PropertyInfo propertyInfo)
        {
            bool parsedExpression = EvaluateCondition(expression);
            propertyInfo.SetValue(collection, parsedExpression);
        }

        private void ParseInt(
            string propertyName, string expression,
            IParseOperator parseOperator, PropertyInfo propertyInfo
        )
        {
            NumberExpression num = parser.ParseNumber(expression);
            double parsedExpression = num.GetNumber();

            ExpressionVariable expressionVariable = parser.ExpressionContext[propertyName];
            double storedVariableNumber = expressionVariable.GetNumber();

            propertyInfo.SetValue(collection, (int)parseOperator.Evaluate(storedVariableNumber, parsedExpression));
        }

        private bool IsAssignment(string expression, out string oper, out IParseOperator parseOperator)
        {
            oper = string.Empty;
            parseOperator = null;
            if (expression.Contains(ParseConstants.Equal))
            {
                oper = ParseConstants.Equal;
                parseOperator = new EqualityParser();
                return true;
            }
            else if (expression.Contains(ParseConstants.PlusEqual))
            {
                oper = ParseConstants.PlusEqual;
                parseOperator = new PlusEqualParser();
                return true;
            }
            else if (expression.Contains(ParseConstants.MinusEqual))
            {
                oper = ParseConstants.MinusEqual;
                parseOperator = new MinusEqualParser();
                return true;
            }
            else if (expression.Contains(ParseConstants.TimesEqual))
            {
                oper = ParseConstants.TimesEqual;
                parseOperator = new TimesEqualParser();
                return true;
            }
            else if (expression.Contains(ParseConstants.DivEqual))
            {
                oper = ParseConstants.DivEqual;
                parseOperator = new DivEqualParser();
                return true;
            }

            return false;
        }
    }
}