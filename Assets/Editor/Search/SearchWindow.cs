using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Search
{
    public class SearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView dialogueGraphView;
        private Texture2D indentIcon;

        public void Initialize(DialogueGraphView dialogueGraphView)
        {
            this.dialogueGraphView = dialogueGraphView;

            indentIcon = new Texture2D(1, 1);
            indentIcon.SetPixel(0, 0, Color.clear);
            indentIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<Type> nodeTypes = TypeExtensions.GetTypes<BaseNode>()
                .Except(new Type[] { typeof(StartNode), typeof(EndNode) }).ToList();

            string contextElementTitle;
            List<SearchTreeEntry> dialogueTreeEntries = new List<SearchTreeEntry>();
            foreach (Type nodeType in nodeTypes)
            {

                contextElementTitle = nodeType.Name;
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent($"Add {contextElementTitle}", indentIcon))
                {
                    level = 2,
                    userData = nodeType
                };

                dialogueTreeEntries.Add(entry);
            }

            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new SearchTreeEntry(new GUIContent("Group", indentIcon))
                {
                    level = 2,
                    userData = new Group()
                },
                new SearchTreeGroupEntry(new GUIContent("Simple Logic"), 1),

            };

            searchTreeEntries.AddRange(dialogueTreeEntries);

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            dialogueGraphView.CreateNode(
                dialogueGraphView.GetLocalMousePosition(context.screenMousePosition), 
                (Type)SearchTreeEntry.userData
            );
            return true;
        }
    }
}
