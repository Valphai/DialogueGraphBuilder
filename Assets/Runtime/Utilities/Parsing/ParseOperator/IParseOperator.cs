namespace Chocolate4.Runtime.Utilities.Parsing
{
    public interface IParseOperator
    {
        double Evaluate(double original, double parsedExpression);
    }

    public class EqualityParser : IParseOperator
    {
        public double Evaluate(double original, double parsedExpression) => parsedExpression;
    }

    public class PlusEqualParser : IParseOperator
    {
        public double Evaluate(double original, double parsedExpression) => original + parsedExpression;
    }

    public class MinusEqualParser : IParseOperator
    {
        public double Evaluate(double original, double parsedExpression) => original - parsedExpression;
    }

    public class TimesEqualParser : IParseOperator
    {
        public double Evaluate(double original, double parsedExpression) => original * parsedExpression;
    }

    public class DivEqualParser : IParseOperator
    {
        public double Evaluate(double original, double parsedExpression) => original / parsedExpression;
    }
}