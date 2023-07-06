using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Dialogue.Tests.Utilities;
using NUnit.Framework;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Tests.PlayMode
{
    internal class MasterTests
    {
        private const string Test = "Test";
        private const string Test1 = "Test (1)";
        private const string Test2 = "Test (2)";
        private const string Test3 = "Test (3)";
        private const string Test4 = "Test (4)";
        private const string Test5 = "Test (5)";
        private const string Test6 = "Test (6)";
        private const string Test7 = "Test (7)";

        private DialogueMaster dialogueMaster;

        [SetUp]
        public void SetUp()
        {
            dialogueMaster = DialogueAssetSetup.LoadMaster();
        }

        [Test]
        public void EndNode_Returns_SituationEndedInfo()
        {
            dialogueMaster.StartSituation(Test);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.SituationEnded);
        }
        
        [Test]
        public void Choice_Node_Returns_ChoiceNodeInfo()
        {
            dialogueMaster.StartSituation(Test1);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.IsChoiceNode);
        }

        [Test]
        public void Dialogue_Node_Returns_DialogueNodeInfo()
        {
            dialogueMaster.StartSituation(Test2);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.IsDialogueNode);
            Assert.IsTrue(!nextInfo.DialogueText.Equals(string.Empty));
        }

        [Test]
        public void Event_Node_Raises_Event()
        {
            dialogueMaster.StartSituation(Test7);

            TestCasesDialogueEditorCollection collection = (TestCasesDialogueEditorCollection)dialogueMaster.Collection;

            List<int> receivedEvents = new List<int>();
            collection.MyEvent += Collection_MyEvent;

            void Collection_MyEvent()
            {
                receivedEvents.Add(1);
                collection.MyEvent -= Collection_MyEvent;
            }

            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();
            Assert.IsTrue(!receivedEvents.IsNullOrEmpty());
        }


        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Condition_Node_Evaluates_Correctly(int value)
        {
            dialogueMaster.StartSituation(Test3);
            TestCasesDialogueEditorCollection collection = (TestCasesDialogueEditorCollection)dialogueMaster.Collection;

            collection.MyInt = value;
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            if (value == 1)
            {
                Assert.IsTrue(nextInfo.DialogueText.Equals("True"));
            }
            else
            {
                Assert.IsTrue(nextInfo.DialogueText.Equals("False"));
            }
        }
        
        [Test]
        public void Expression_Node_Evaluates_Correctly()
        {
            dialogueMaster.StartSituation(Test4);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();
            TestCasesDialogueEditorCollection collection = (TestCasesDialogueEditorCollection)dialogueMaster.Collection;

            Assert.IsTrue(collection.MyBool == false);
            Assert.IsTrue(collection.MyInt == 3);
        }
        
        [Test]
        public void To_Situation_Node_Changes_Situation()
        {
            dialogueMaster.StartSituation(Test5);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(dialogueMaster.CurrentSituationName.Equals(Test6));
            Assert.IsTrue(nextInfo.SituationEnded);
        }

        [Test]
        public void Selecting_Choice_Ensures_Continuity()
        {
            dialogueMaster.StartSituation(Test1);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.IsChoiceNode);

            dialogueMaster.SetSelectedChoice(1);
            nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.SituationEnded);
        }

        [Test]
        [TestCase(Test)]
        [TestCase(Test1)]
        [TestCase(Test2)]
        public void Can_Switch_Situations(string situationName)
        {
            dialogueMaster.StartSituation(Test);
            dialogueMaster.NextDialogueElement();
            Assert.IsTrue(dialogueMaster.CurrentSituationName.Equals(Test));

            dialogueMaster.StartSituation(situationName);
            Assert.IsTrue(dialogueMaster.CurrentSituationName.Equals(situationName));
        }
    }
}