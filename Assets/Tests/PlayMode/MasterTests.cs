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
        private DialogueMaster dialogueMaster;

        [SetUp]
        public void SetUp()
        {
            dialogueMaster = DialogueAssetSetup.LoadMaster();
        }

        [Test]
        public void EndNode_Returns_SituationEndedInfo()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.SituationEnded);
        }
        
        [Test]
        public void Choice_Node_Returns_ChoiceNodeInfo()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test1);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.IsChoiceNode);
        }

        [Test]
        public void Dialogue_Node_Returns_DialogueNodeInfo()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test2);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.IsDialogueNode);
            Assert.IsTrue(!nextInfo.DialogueText.Equals(string.Empty));
        }

        [Test]
        public void Event_Node_Raises_Event()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test7);

            TestCasesDialogueEditorCollection collection = 
                dialogueMaster.GetCollection<TestCasesDialogueEditorCollection>();

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
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test3);
            TestCasesDialogueEditorCollection collection = 
                dialogueMaster.GetCollection<TestCasesDialogueEditorCollection>();

            collection.MyInt = value;
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.DialogueText.Equals(value == 1 ? "True" : "False"));
        }
        
        [Test]
        public void Expression_Node_Evaluates_Correctly()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test4);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();
            TestCasesDialogueEditorCollection collection = 
                dialogueMaster.GetCollection<TestCasesDialogueEditorCollection>();

            Assert.IsTrue(collection.MyBool == false);
            Assert.IsTrue(collection.MyInt == 3);
        }
        
        [Test]
        public void To_Situation_Node_Changes_Situation()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test5);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(dialogueMaster.CurrentSituationName.Equals(TestCasesDialogueEditorCollection.Test6));
            Assert.IsTrue(nextInfo.SituationEnded);
        }

        [Test]
        public void Selecting_Choice_Ensures_Continuity()
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test1);
            DialogueNodeInfo nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.IsChoiceNode);

            dialogueMaster.SetSelectedChoice(1);
            nextInfo = dialogueMaster.NextDialogueElement();

            Assert.IsTrue(nextInfo.SituationEnded);
        }

        [Test]
        [TestCase(TestCasesDialogueEditorCollection.Test)]
        [TestCase(TestCasesDialogueEditorCollection.Test1)]
        [TestCase(TestCasesDialogueEditorCollection.Test2)]
        public void Can_Switch_Situations(string situationName)
        {
            dialogueMaster.StartSituation(TestCasesDialogueEditorCollection.Test);
            dialogueMaster.NextDialogueElement();
            Assert.IsTrue(dialogueMaster.CurrentSituationName.Equals(TestCasesDialogueEditorCollection.Test));

            dialogueMaster.StartSituation(situationName);
            Assert.IsTrue(dialogueMaster.CurrentSituationName.Equals(situationName));
        }
    }
}