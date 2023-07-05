using B83.LogicExpressionParser;
using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Tests.Utilities;
using Chocolate4.Runtime.Utilities.Parsing;
using NUnit.Framework;

namespace Chocolate4.Dialogue.Tests.PlayMode
{
    internal class ParserTests
    {
        internal const string MyInt = "MyInt";
        internal const string MyBool = "MyBool";

        private DialogueMaster dialogueMaster;

        [SetUp]
        public void SetUp()
        {
            dialogueMaster = DialogueAssetSetup.LoadMaster();
        }

        [Test]
        public void Parser_Keeps_Track_Of_Variable_In_Collection()
        {
            Parser parser = DialogueAssetSetup.GetParser(dialogueMaster, out ParseAdapter _);

            int original = dialogueMaster.Collection.MyInt;
            dialogueMaster.Collection.MyInt += 1;

            Assert.IsTrue(original != dialogueMaster.Collection.MyInt);

            Assert.IsTrue(parser.ExpressionContext[MyInt].GetNumber() == dialogueMaster.Collection.MyInt);
        }
    }
}