using Chocolate4.Dialogue.Edit;
using Chocolate4.Dialogue.Edit.CodeGeneration;
using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Asset;
using System;
using UnityEngine;

namespace Chocolate4.Dialogue.Tests.Utilities
{
    public static class GraphViewUtilities
    {
        public static DialogueEditorWindow MakeAssetAndOpenWindow(DialogueEditorAsset dialogueAsset)
        {
            Writer writer = DialogueFileMenuItem.WriteDefaultContent();
            dialogueAsset.LoadFromJson(writer.buffer.ToString());

            return DialogueEditorWindow.OpenEditor(dialogueAsset, dialogueAsset.GetInstanceID());
        }

        public static void MakeNodes(
            DialogueEditorWindow window,
            Type thisNodeType, Type otherNodeType,
            out BaseNode thisNode, out BaseNode otherNode
        )
        {
            thisNode = window.GraphView.CreateNode(Vector3.zero, thisNodeType);
            otherNode = window.GraphView.CreateNode(Vector3.zero, otherNodeType);

            return;
        }
    }
}
