using Chocolate4.Saving;
using Chocolate4.Tree;
using Chocolate4.Utilities;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4
{
    public class DialogueEditorWindow : EditorWindow
    {
        public const string ContainerStyle = "tree-view__content-container";
        public const string ButtonBigStyle = "tree-view__content-button-big";

        //private ListView leftListView;
        private DialogueGraphView graphView;
        private DialogueTreeView dialogueTreeView;
        [SerializeField]
        private DialogueAssetManager dialogueAssetManager;
        [SerializeField] 
        private bool hasInitialized;

        private TwoPaneSplitView splitView;
        private VisualElement leftPanel;
        private Button saveButton;

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            if (!path.EndsWith(DialogueImporter.Extension, StringComparison.InvariantCultureIgnoreCase))
                return false;

            //string mapToSelect = null;
            //string actionToSelect = null;

            // Grab InputActionAsset.
            // NOTE: We defer checking out an asset until we save it. This allows a user to open an .inputactions asset and look at it
            //       without forcing a checkout.
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            DialogueEditorAsset asset = obj as DialogueEditorAsset;
            if (asset == null)
            {
                // Check if the user clicked on an action inside the asset.
                //var actionReference = obj as InputActionReference;
                //if (actionReference != null)
                //{
                //    asset = actionReference.asset;
                //    mapToSelect = actionReference.action.actionMap.name;
                //    actionToSelect = actionReference.action.name;
                //}
                //else
                    return false;
            }

            var window = OpenEditor(asset, instanceId);

            // If user clicked on an action inside the asset, focus on that action (if we can find it).
            //if (actionToSelect != null && window.m_ActionMapsTree.TrySelectItem(mapToSelect))
            //{
            //    window.OnActionMapTreeSelectionChanged();
            //    window.m_ActionsTree.SelectItem(actionToSelect);
            //}

            return true;
        }

        public static DialogueEditorWindow OpenEditor(DialogueEditorAsset asset, int instanceId)
        {
            ////REVIEW: It'd be great if the window got docked by default but the public EditorWindow API doesn't allow that
            ////        to be done for windows that aren't singletons (GetWindow<T>() will only create one window and it's the
            ////        only way to get programmatic docking with the current API).
            // See if we have an existing editor window that has the asset open.
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("DialogueEditorWindow");
            window.SetAsset(asset, instanceId);

            window.Show();
            window.Focus();

            return window;
        }

        private void SetAsset(DialogueEditorAsset asset, int instanceId)
        {
            if (asset == null)
                return;

            dialogueAssetManager = new DialogueAssetManager(asset, instanceId);

            //m_ActionAssetManager = new InputActionAssetManager(asset) { onDirtyChanged = OnDirtyChanged };
            //m_ActionAssetManager.Initialize();

            PostInitialize();

            //InitializeTrees();
            //LoadControlSchemes();

            // Select first action map in asset.
            //m_ActionMapsTree.SelectFirstToplevelItem();

            //UpdateWindowTitle();
        }

        private void OnEnable()
        {
            if (!hasInitialized)
            {
                return;
            }

            Initialize();
        }

        private void OnDisable()
        {
            dialogueTreeView.OnSituationSelected -= graphView.DialogueTreeView_OnSituationSelected;
            StoreData();
        }

        private void StoreData()
        {
            TreeSaveData treeSaveData = dialogueTreeView.Save();
            GraphSaveData graphSaveData = graphView.Save();
            dialogueAssetManager.Store(graphSaveData, treeSaveData);
        }

        private void Initialize()
        {
            CreateElements();

            Rebuild();

            AddStyles();
            DrawPanels();
        }

        private void PostInitialize()
        {
            Initialize();
            //dialogueTreeView = new DialogueTreeView(dialogueAssetManager.ImportedAsset.treeSaveData);
            //dialogueTreeView.OnSituationSelected += graphView.DialogueTreeView_OnSituationSelected;

            hasInitialized = true;
        }

        private void CreateElements()
        {
            leftPanel = new VisualElement();

            graphView = new DialogueGraphView();
            graphView.Initialize();
            dialogueTreeView = new DialogueTreeView();
            dialogueTreeView.Initialize(dialogueAssetManager.ImportedAsset.treeSaveData);

            dialogueTreeView.OnSituationSelected += graphView.DialogueTreeView_OnSituationSelected;
        }

        private void Rebuild()
        {
            dialogueAssetManager.Rebuild(dialogueTreeView, graphView);
        }

        private void DrawPanels()
        {
            rootVisualElement.Clear();

            splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            splitView.Add(leftPanel);
            splitView.Add(graphView);

            rootVisualElement.Add(splitView);
            
            AddToolbar();
            AddHeaderColumns();
            AddTreeView();
            AddListView();
        }

        private void AddTreeView()
        {
            leftPanel.Add(dialogueTreeView.TreeView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            ToolbarSearchField searchField = new ToolbarSearchField();

            saveButton = new Button(){
                text = "Save"
            };

            saveButton.clicked += SaveButton_clicked;

            toolbar.Add(searchField);
            toolbar.Add(saveButton);

            leftPanel.Add(toolbar);
        }

        private void SaveButton_clicked()
        {
            TreeSaveData treeData = dialogueTreeView.Save();
            GraphSaveData graphData = graphView.Save();
            dialogueAssetManager.Save(graphData, treeData);
        }

        private void AddListView()
        {
            //leftListView = new ListView();

            //leftPanel.makeItem = () => new Label();
            //leftPanel.bindItem = (item, index) => { (item as Label).text = factory.Npcs[index].characterName; };
            //leftPanel.itemsSource = (System.Collections.IList)factory.Npcs;

            //leftPanel.Add(leftListView);
        }

        private void AddHeaderColumns()
        {
            VisualElement buttonsContainer = new VisualElement().WithStyle(ContainerStyle);

            buttonsContainer.WithButton(TreeGroupsExtensions.SituationString).WithOnClick(
                () => dialogueTreeView.AddTreeItem(
                    TreeGroupsExtensions.DefaultSituationName, TreeGroups.Situation, TreeItemType.Group
                )
            );

            buttonsContainer.WithButton(TreeGroupsExtensions.VariableGroupString).WithOnClick(
                () => dialogueTreeView.AddTreeItem(
                    TreeGroupsExtensions.DefaultVariableGroupName, TreeGroups.Variable, TreeItemType.Group
                )
            );

            buttonsContainer.WithButton(TreeGroupsExtensions.EventGroupString).WithOnClick(
            () => dialogueTreeView.AddTreeItem(
                    TreeGroupsExtensions.DefaultEventGroupName, TreeGroups.Event, TreeItemType.Group
                )
            );

            leftPanel.Add(buttonsContainer);
        }

        private void AddStyles()
        {
            StyleSheet elementsStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueElementsStyle.uss");

            leftPanel.styleSheets.Add(elementsStyleSheet);
        }
    }
}
