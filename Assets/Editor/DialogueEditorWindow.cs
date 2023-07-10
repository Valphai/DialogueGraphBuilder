using Chocolate4.Dialogue.Edit.Asset;
using Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger;
using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Entities;
using Chocolate4.Edit.Graph;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit
{
    public class DialogueEditorWindow : EditorWindow
    {
        public static DialogueEditorWindow Window { get; private set; }

        [SerializeField] 
        private bool hasInitialized;

        private TwoPaneSplitView splitView;
        private VisualElement mainSplitView;
        private VisualElement subTwoPanelView;
        private VisualElement subPanelSituationsContent;
        private VisualElement subPanelEntitiesContent;

        private Button saveButton;
        private DialogueAssetManager dialogueAssetManager;

        public DialogueEntitiesView EntitiesView { get; private set; }
        public DialogueGraphView GraphView { get; private set; }
        public DialogueTreeView DialogueTreeView { get; private set; }

        [SerializeField]
        private int selectedColumnIndex = 0;

        private int SelectedColumnIndex
        {
            get => selectedColumnIndex;
            set
            {
                if (selectedColumnIndex == value)
                {
                    return;
                }

                selectedColumnIndex = value;
                OpenSelectedView();
            }
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            if (!path.EndsWith(FilePathConstants.Extension, StringComparison.InvariantCultureIgnoreCase))
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

            string entityDatabasePath = Directory.GetFiles(
                FilePathConstants.GetPathRelativeTo(FilePathConstants.Assets, FilePathConstants.dialogueEntitiesPath), "*.asset"
            ).First(path => path.Contains(EntitiesHolder.DataBase));

            EntitiesHolder entitiesDatabase = AssetDatabase.LoadAssetAtPath<EntitiesHolder>(entityDatabasePath);

            dialogueAssetManager = new DialogueAssetManager(asset, instanceId, entitiesDatabase);

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

        private void TrySave()
        {
            if (DangerLogger.IsEditorInDanger())
            {
                return;
            }

            GraphSaveData graphData = GraphView.Save();
            TreeSaveData treeData = DialogueTreeView.Save();
            EntitiesData entitiesData = EntitiesView.Save();
            dialogueAssetManager.Save(graphData, treeData, entitiesData);
        }

        private void StoreData()
        {
            TreeSaveData treeSaveData = DialogueTreeView.Save();
            GraphSaveData graphSaveData = GraphView.Save();
            EntitiesData entitiesData = EntitiesView.Save();
            dialogueAssetManager.Store(graphSaveData, treeSaveData, entitiesData);
        }

        private void Initialize()
        {
            CreateElements();

            Rebuild();

            CreatePanels();

            OpenSelectedView();
        }

        private void PostInitialize()
        {
            Initialize();
            hasInitialized = true;
        }

        private void CreateElements()
        {
            EntitiesView = new DialogueEntitiesView();
            EntitiesView.Initialize();

            EntitiesView.WithFlexGrow().WithFlexShrink(1f);

            mainSplitView = new VisualElement();
            subTwoPanelView = new VisualElement();
            subPanelSituationsContent = new VisualElement().WithFlexGrow().WithFlexShrink(1f);
            subPanelEntitiesContent = new VisualElement().WithFlexGrow().WithFlexShrink(1f);

            GraphView = new DialogueGraphView();
            GraphView.Initialize();
            GraphView.WithFlexGrow();
            DialogueTreeView = new DialogueTreeView();
            DialogueTreeView.Initialize(dialogueAssetManager.ImportedAsset.treeSaveData);

            DialogueTreeView.OnSituationSelected += GraphView.DialogueTreeView_OnSituationSelected;
            DialogueTreeView.OnTreeItemRemoved += GraphView.DialogueTreeView_OnTreeItemRemoved;

            GraphView.SituationCache.OnSituationCached += DialogueTreeView.GraphView_OnSituationCached;
        }

        private void Rebuild()
        {
            dialogueAssetManager.Rebuild(DialogueTreeView, GraphView, EntitiesView);
        }

        private void CreatePanels()
        {
            rootVisualElement.Clear();

            AddSplitView();
            AddToolbar();
            AddHeaderColumns();
            AddGraphHeaderButtons();
            AddEntitiesHeaderButtons();
            AddTreeView();
            AddListView();
        }

        private void AddSplitView()
        {
            splitView = new TwoPaneSplitView(1, GraphConstants.TreeViewWindowWidth, TwoPaneSplitViewOrientation.Horizontal);

            splitView.Add(mainSplitView);
            splitView.Add(subTwoPanelView);

            mainSplitView.Add(GraphView);
            mainSplitView.Add(EntitiesView);

            rootVisualElement.Add(splitView);
        }

        private void AddHeaderColumns()
        {
            IMGUIContainer container = new IMGUIContainer(() => {

                EditorGUILayout.BeginHorizontal();

                SelectedColumnIndex = GUILayout.Toolbar(SelectedColumnIndex, new string[] { "Situations", "Entities" });

                EditorGUILayout.EndHorizontal();
            });

            subTwoPanelView.Add(container);

            subTwoPanelView.Add(subPanelSituationsContent);
            subTwoPanelView.Add(subPanelEntitiesContent);

            SelectSituationView();
        }

        private void AddTreeView()
        {
            subPanelSituationsContent.Add(DialogueTreeView.TreeView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            ToolbarSearchField searchField = new ToolbarSearchField();
            searchField.WithFlexGrow()
                .WithFlexShrink(1f)
                .WithMinWidth(GraphConstants.SaveButtonWidth);

            saveButton = new Button() {
                text = "Save"
            }.WithOnClick(TrySave);
            saveButton.WithMaxWidth(GraphConstants.SaveButtonWidth);

            toolbar.Add(searchField);
            toolbar.Add(saveButton);

            subTwoPanelView.Add(toolbar);
        }

        private void AddListView()
        {
            subPanelEntitiesContent.Add(EntitiesView.ListView);
        }

        private void AddEntitiesHeaderButtons()
        {
            VisualElement buttonsContainer = new VisualElement().WithHorizontalGrow();

            Button addButton = buttonsContainer.WithButton("Add Entity").WithOnClick(
                () => EntitiesView.AddEntity()
            );

            addButton
                .WithFlexGrow()
                .WithMinWidth(GraphConstants.InsertButtonWidth);

            subPanelEntitiesContent.Add(buttonsContainer);
        }

        private void AddGraphHeaderButtons()
        {
            VisualElement buttonsContainer = new VisualElement().WithHorizontalGrow();

            buttonsContainer.WithButton(TreeGroupsExtensions.SituationString).WithOnClick(
                () => {
                    DialogueTreeItem item = DialogueTreeView.AddTreeItem(
                        TreeGroupsExtensions.DefaultSituationName, TreeGroups.Situation, TreeItemType.Group
                    );

                    GraphView.SituationCache.TryCache(new SituationSaveData(item.id, null));
                }
            ).WithMinWidth(GraphConstants.InsertButtonWidth);

            buttonsContainer.WithButton(TreeGroupsExtensions.VariableGroupString).WithOnClick(
                () => DialogueTreeView.AddTreeItem(
                    TreeGroupsExtensions.DefaultVariableGroupName, TreeGroups.Variable, TreeItemType.Group
                )
            ).WithMinWidth(GraphConstants.InsertButtonWidth);

            buttonsContainer.WithButton(TreeGroupsExtensions.EventGroupString).WithOnClick(
            () => DialogueTreeView.AddTreeItem(
                    TreeGroupsExtensions.DefaultEventGroupName, TreeGroups.Event, TreeItemType.Group
                )
            ).WithMinWidth(GraphConstants.InsertButtonWidth);

            subPanelSituationsContent.Add(buttonsContainer);
        }

        private void SelectEntityView()
        {
            SelectSplitPanelView(
                EntitiesView, subPanelEntitiesContent,
                GraphView, subPanelSituationsContent
            );
        }

        private void SelectSituationView()
        {
            SelectSplitPanelView(
                GraphView, subPanelSituationsContent,
                EntitiesView, subPanelEntitiesContent
            );
        }

        private void SelectSplitPanelView(
            VisualElement view, VisualElement subPanel,
            VisualElement otherView, VisualElement otherSubPanel
        )
        {
            if (mainSplitView.Contains(otherView))
            {
                mainSplitView.Remove(otherView);
            }
            mainSplitView.Insert(0, view);

            view.style.display = DisplayStyle.Flex;
            subPanel.style.display = DisplayStyle.Flex;

            otherView.style.display = DisplayStyle.None;
            otherSubPanel.style.display = DisplayStyle.None;
        }

        private void OpenSelectedView()
        {
            if (selectedColumnIndex == GraphConstants.SituationViewIndex)
            {
                SelectSituationView();
            }
            else if (selectedColumnIndex == GraphConstants.EntityViewIndex)
            {
                SelectEntityView();
            }
        }
    }
}
