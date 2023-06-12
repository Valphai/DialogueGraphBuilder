using Chocolate4.Dialogue.Edit.Asset;
using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit
{
    public class DialogueEditorWindow : EditorWindow
    {
        public const string ContainerStyle = "tree-view__content-container";
        public const string ButtonBigStyle = "tree-view__content-button-big";

        public static DialogueEditorWindow Window { get; private set; }

        [SerializeField] 
        private bool hasInitialized;

        //private ListView leftListView;
        private TwoPaneSplitView splitView;
        private VisualElement leftPanel;
        private Button saveButton;
        private DialogueAssetManager dialogueAssetManager;

        public DialogueGraphView GraphView { get; private set; }
        public DialogueTreeView DialogueTreeView { get; private set; }

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

            Window = OpenEditor(asset, instanceId);

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

            Window = this;
            Initialize();
        }

        private void OnDisable()
        {
            DialogueTreeView.OnSituationSelected -= GraphView.DialogueTreeView_OnSituationSelected;
            DialogueTreeView.OnTreeItemRemoved -= GraphView.DialogueTreeView_OnTreeItemRemoved;

            GraphView.SituationCache.OnSituationCached -= DialogueTreeView.GraphView_OnSituationCached;

            StoreData();
        }

        private void StoreData()
        {
            TreeSaveData treeSaveData = DialogueTreeView.Save();
            GraphSaveData graphSaveData = GraphView.Save();
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
            hasInitialized = true;
        }

        private void CreateElements()
        {
            leftPanel = new VisualElement();

            GraphView = new DialogueGraphView();
            GraphView.Initialize();
            DialogueTreeView = new DialogueTreeView();
            DialogueTreeView.Initialize(dialogueAssetManager.ImportedAsset.treeSaveData);

            DialogueTreeView.OnSituationSelected += GraphView.DialogueTreeView_OnSituationSelected;
            DialogueTreeView.OnTreeItemRemoved += GraphView.DialogueTreeView_OnTreeItemRemoved;

            GraphView.SituationCache.OnSituationCached += DialogueTreeView.GraphView_OnSituationCached;
        }

        private void Rebuild()
        {
            dialogueAssetManager.Rebuild(DialogueTreeView, GraphView);
        }

        private void DrawPanels()
        {
            rootVisualElement.Clear();

            splitView = new TwoPaneSplitView(1, GraphConstants.TreeViewWindowWidth, TwoPaneSplitViewOrientation.Horizontal);
            splitView.Add(GraphView);
            splitView.Add(leftPanel);

            rootVisualElement.Add(splitView);

            AddToolbar();
            AddHeaderColumns();
            AddTreeView();
            AddListView();
        }

        private void AddTreeView()
        {
            leftPanel.Add(DialogueTreeView.TreeView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            ToolbarSearchField searchField = new ToolbarSearchField();

            searchField.style.width = GraphConstants.SearchBarWidth;

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
            GraphSaveData graphData = GraphView.Save();
            TreeSaveData treeData = DialogueTreeView.Save();
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
                () => {
                    DialogueTreeItem item = DialogueTreeView.AddTreeItem(
                        TreeGroupsExtensions.DefaultSituationName, TreeGroups.Situation, TreeItemType.Group
                    );

                    GraphView.SituationCache.TryCache(new SituationSaveData(item.guid, null));
                }
            );

            buttonsContainer.WithButton(TreeGroupsExtensions.VariableGroupString).WithOnClick(
                () => DialogueTreeView.AddTreeItem(
                    TreeGroupsExtensions.DefaultVariableGroupName, TreeGroups.Variable, TreeItemType.Group
                )
            );

            buttonsContainer.WithButton(TreeGroupsExtensions.EventGroupString).WithOnClick(
            () => DialogueTreeView.AddTreeItem(
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
