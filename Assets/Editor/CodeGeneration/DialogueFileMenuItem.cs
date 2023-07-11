using Chocolate4.Dialogue.Edit.Asset;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.CodeGeneration
{
    public static class DialogueFileMenuItem
    {
        private const string FileName = "DialogueEditor";

        [MenuItem("Assets/Create/DialogueEditor")]
        public static void MakeDialogueEditorFile()
        {
            Writer writer = WriteDefaultContent();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject) + FilePathConstants.DirSep + FileName + "." + FilePathConstants.Extension;

            string content = writer.buffer.ToString();
            ProjectWindowUtil.CreateAssetWithContent(path, content, (Texture2D)EditorGUIUtility.Load(FilePathConstants.assetIcon));

            AssetDatabase.Refresh();
        }

        public static Writer WriteDefaultContent()
        {
            string situationId = Guid.NewGuid().ToString();
            string startNodeId = Guid.NewGuid().ToString();
            string endNodeId = Guid.NewGuid().ToString();

            StringBuilder sb = new StringBuilder(
                "{\r\n    \"collectionName\": \"None\",\r\n    \"graphSaveData\": {\r\n        \"graphViewPosition\": {\r\n            \"x\": 119.0,\r\n            \"y\": 7.0\r\n        },\r\n        \"graphViewZoom\": {\r\n            \"x\": 0.7561436891555786,\r\n            \"y\": 0.7561436891555786\r\n        },\r\n        \"situationSaveData\": [\r\n            {\r\n                \"situationId\": \"639f4b98-c5e6-4cb0-a74a-b656cb516d6c\",\r\n                \"nodeData\": [\r\n                    {\r\n                        \"nodeId\": \"18e55f56-3081-45fd-bef0-e4113b52b015\",\r\n                        \"nodeType\": \"Chocolate4.Dialogue.Edit.Graph.Nodes.StartNode\",\r\n                        \"position\": {\r\n                            \"x\": 139.5,\r\n                            \"y\": 312.0\r\n                        },\r\n                        \"groupId\": \"\",\r\n                        \"inputPortDataCollection\": [],\r\n                        \"outputPortDataCollection\": [\r\n                            {\r\n                                \"otherNodeID\": \"\",\r\n                                \"otherPortName\": \"\",\r\n                                \"thisPortName\": \"Out\",\r\n                                \"thisPortType\": \"Chocolate4.Edit.Graph.Utilities.TransitionPortType\"\r\n                            }\r\n                        ]\r\n                    },\r\n                    {\r\n                        \"nodeId\": \"cf971c5f-7f4a-4b58-b056-cb9f8c18201a\",\r\n                        \"nodeType\": \"Chocolate4.Dialogue.Edit.Graph.Nodes.EndNode\",\r\n                        \"position\": {\r\n                            \"x\": 539.5,\r\n                            \"y\": 312.0\r\n                        },\r\n                        \"groupId\": \"\",\r\n                        \"inputPortDataCollection\": [\r\n                            {\r\n                                \"otherNodeID\": \"\",\r\n                                \"otherPortName\": \"\",\r\n                                \"thisPortName\": \"In\",\r\n                                \"thisPortType\": \"Chocolate4.Edit.Graph.Utilities.TransitionPortType\"\r\n                            }\r\n                        ],\r\n                        \"outputPortDataCollection\": []\r\n                    }\r\n                ],\r\n                \"textNodeData\": [],\r\n                \"dialogueNodeData\": [],\r\n                \"propertyNodeSaveData\": [],\r\n                \"situationTransferNodeData\": [],\r\n                \"choiceNodeData\": []\r\n            }\r\n        ],\r\n        \"blackboardSaveData\": {\r\n            \"dialoguePropertiesSaveData\": []\r\n        }\r\n    },\r\n    \"treeSaveData\": {\r\n        \"selectedIndex\": 0,\r\n        \"treeItemData\": [\r\n            {\r\n                \"depth\": 0,\r\n                \"rootItem\": {\r\n                    \"displayName\": \"New Situation\",\r\n                    \"id\": \"639f4b98-c5e6-4cb0-a74a-b656cb516d6c\",\r\n                    \"prefix\": \"S\"\r\n                },\r\n                \"childrenGuids\": []\r\n            }\r\n        ]\r\n    }\r\n}"
            );

            sb.Replace("18e55f56-3081-45fd-bef0-e4113b52b015", startNodeId);
            sb.Replace("cf971c5f-7f4a-4b58-b056-cb9f8c18201a", endNodeId);
            sb.Replace("639f4b98-c5e6-4cb0-a74a-b656cb516d6c", situationId);
            return new Writer(sb);
        }
    }
}
