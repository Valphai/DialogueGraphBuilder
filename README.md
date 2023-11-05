![](Assets/Documentation/Images/logo.png)

![GitHub last commit (by committer)](https://img.shields.io/github/last-commit/Valphai/DialogueGraphBuilder)
![GitHub](https://img.shields.io/github/license/Valphai/DialogueGraphBuilder)
![GitHub release (with filter)](https://img.shields.io/github/v/release/Valphai/DialogueGraphBuilder)
![GitHub Release Date](https://img.shields.io/github/release-date/Valphai/DialogueGraphBuilder)

# **Table of contents**

- [**Table of contents**](#table-of-contents)
- [**Introduction**](#introduction)
- [**Key Features**](#key-features)
- [**Installation**](#installation)
- [**Quick Setup**](#quick-setup)
  - [**Editor**](#editor)
  - [**Build/Playmode**](#buildplaymode)
- [**Documentation**](#documentation)
  - [**Situations**](#situations)
  - [**Entities**](#entities)
    - [**IEntityProcessor**](#ientityprocessor)
  - [**DialogueMaster**](#dialoguemaster)
      - [```Initialize()```](#initialize)
      - [```StartSituation(string situationName)```](#startsituationstring-situationname)
      - [```NextDialogueElement()```](#nextdialogueelement)
      - [```SetSelectedChoice(int index)```](#setselectedchoiceint-index)
      - [```GetCollection<T>()```](#getcollectiont)
    - [**Entities**](#entities-1)
    - [**Collections**](#collections)
      - [**Automatic Generation**](#automatic-generation)
      - [**Accessing Collections**](#accessing-collections)
      - [**Runtime Creation**](#runtime-creation)
      - [**Managing Variables and Events**](#managing-variables-and-events)
  - [**DialogueNodeInfo**](#dialoguenodeinfo)
  - [**Graph View**](#graph-view)
    - [**Blackboard**](#blackboard)
    - [**Nodes**](#nodes)
      - [**Dialogue Node**](#dialogue-node)
      - [**Choice Node**](#choice-node)
      - [**Expression Node**](#expression-node)
      - [**Condition Node**](#condition-node)
      - [**Start Node**](#start-node)
      - [**End Node**](#end-node)
      - [**From Situation Node**](#from-situation-node)
      - [**To Situation Node**](#to-situation-node)
      - [**Event Node**](#event-node)
- [**Examples**](#examples)
  - [**Princess and the King**](#princess-and-the-king)
  - [**Guard at the gate**](#guard-at-the-gate)
  - [**Shopkeeper**](#shopkeeper)
- [**Release Notes**](#release-notes)
  - [**Version 1.0.0**](#version-100)
- [**Additional Notes**](#additional-notes)
- [**Credits**](#credits)
    - [Additional credits for art assets used in screenshots](#additional-credits-for-art-assets-used-in-screenshots)

# **Introduction**

Welcome to DialogueGraphBuilder, a lightweight Unity tool designed to help you create intricate dialogue flows, dialogue entities, variables, and events, all with a focus on modularity. This open-source tool is completely free, making it suitable for both commercial and non-commercial projects.

DialogueGraphBuilder has been extensively tested in Unity on various platforms, including
- PC/Linux
- Android
- WebGL

# **Key Features**

- **Self-contained Editor Window:** Access all the tools you need in a dedicated EditorWindow.
- **User-Defined Situations:** Create and organize unique situations to structure your dialogue.
- **User-Defined Entity Maker:** Define and customize entities for your dialogue, complete with images and names.
- **Shader Graph-Like Dialogue Graph Editor:** Construct dialogue flows using a user-friendly and visually intuitive editor.
- **Dialogue Nodes:** Easily add dialogue nodes with text and entity selection options.
- **Choice Nodes:** Present player choices with branching options for dynamic dialogues.
- **Transfer Nodes:** Shift the flow to different situations for seamless transitions, promoting modularity.
- **Tree Hierarchy Window:** Keep your dialogues organized with a hierarchical view of multiple situations.
- **Search Toolbar:** Quickly locate specific situations and created entities.
- **User-Defined Variables:** Create integer and boolean variables that can be used in both the editor and your C# code.
- **Condition Nodes:** Redirect the dialogue flow based on user-defined variables and conditions.
- **Expression Nodes:** Manipulate user-defined variables using a flexible expression format.
- **Support for Global Events:** Implement global, user-defined events as nodes to trigger C# events.
- **DialogueMaster API:** Use a single DialogueMaster to interact with the tool's capabilities.
- **All-in-One JSON Saved File:** Store your entire project in a single JSON file.
- **Automated C# Script Generation:** Automatically generate C# scripts whenever data updates, simplifying your workflow.
- **Automatic Error Logger:** Receive notifications of any errors before saving to prevent issues.

# **Installation**

DialogueGraphBuilder can be installed in various ways:

1. **From Source:** Download the source code and open the project through Unity hub.
2. **From the Releases Page:** Download the latest release from our Releases page for the most up-to-date version.
3. **From the Asset Store (Coming Soon):** Keep an eye out for our Asset Store listing.

We recommend downloading the package from the Releases page to ensure you have the latest version of the tool.

# **Quick Setup**

## **Editor**

To get started in the Unity Editor, follow these steps:

1. After installing the package, navigate to **'Create/Chocolate4/DialogueEditor'**.
2. Double-click the asset to open the tool in the Editor.

## **Build/Playmode**

To use the tool in your build or play mode, follow these steps:

1. Create and connect nodes within the dialogue asset.
2. Create a game object in your scene.
3. Add a **[DialogueMaster](#dialoguemaster)** component to the game object.
4. Enable the "Auto Initialize" field in the Inspector.
5. Drag your dialogue asset and its entity database into the exposed fields.

Now, your dialogue asset is ready to be used through the DialogueMaster.

# **Documentation**

Remember to save your work in the Editor with the Save button, as all changes are cached in memory. Unsaved changes will be lost when you close the window.

## **Situations**

A Situation is dialogue nodes flow that allows to handle a given encounter. One can think of them as folders for all the nodes, organized neatly in the hierarchy.

A Situation can have back-to-back conversation between multiple entities, branching out to different situations depending on user defined variables (see **[To Situation Node](#to-situation-node)**).

This workflow is designed to handle one flow at a time, encouraging you to stay organized with your dialogue options.

Below is an example of a situation chain starting from **"Just entered the shop"**. In this example this can be thought of as a folder we arrive at when starting dialogue with a shopkeeper (see **[Shopkeeper](#shopkeeper)**). Depending on certain conditions, the nodes in that situation will redirect the flow to other nested situations.

![](Assets/Documentation/Images/Situations.png)

With the "New Situation" button you can create new situations and reorder them in the TreeView's hierarchy. The search window to the left of the "Save" button allows you to filter created situations by name.

Right-click on any situation to access the Contextual Menu, allowing you to rename or delete a situation.

## **Entities**

Entities are actors interacting with one another through **[Dialogue nodes](#dialogue-node)**, they are ScriptableObject instances of ```DialogueSpeaker```. You can access and modify their Identifier, EntityImage, EntityName as well as extraImages and extraText. All of this information can be accessed at runtime from **[DialogueNodeInfo](#dialoguenodeinfo)** provided by the **[DialogueMaster's](#dialoguemaster)** API.

The "New Entity" button lets you create and edit entities within the Editor window. All entities are stored in memory until you press the "Save" button.

The Identifier is unique for each entity and is shared among clones of the same instance.  Additionally, there is an exposed List of **[IEntityProcessor](#ientityprocessor)** you can use on every entity. You can make your own processors to make use of extra entity data. 

You can also sort entities by name using the button next to "New Entity." Similar to situations, you can filter created entities by name in the search box.

![](Assets/Documentation/Images/EntitiesView.png)

Right-click on any entity to access the Contextual Menu and delete an entity.

For accessing entities at runtime see **[DialogueMaster](#entities-1)**.

### **IEntityProcessor**

IEntityProcessor is an interface that allows to define a collection of behaviours or modifications for an entity. See example usage in **[ShopKeeper](#shopkeeper)**.

Whenever an entity is processed, a clone of the ```DialogueEntity``` is created. The motivation for this is that you can safely edit the cloned scriptable object inside of a custom processor and use the processed data all while having the original entity untouched.

```csharp
public DialogueEntity Process(DialogueMaster master, DialogueEntity speaker)
{
}
```

This processing is especially useful for dynamically changing the image or name of an entity at runtime. In order to process an entity with custom processors, begin by implementing ```IEntityProcessor``` and assign your custom processor to the EntityProcessors list in the EntityView.

Instead of directly using the entity provided by **[DialogueNodeInfo](#dialoguenodeinfo)**, use the following pattern:

```csharp
using (DialogueEntity processedEntity = newSpeaker.Process(dialogueMaster))
{
    // set your presentation objects using processedEntity fields.
}
```

The above approach will create a processed clone of the entity with identifier identical to the original.

## **DialogueMaster**

Dialogue Master is a component responsible for running the dialogue graph flow at runtime. It provides the API to start different situations, retrieve current **[DialogueNodeInfo](#dialoguenodeinfo)** or grant access to variable [collection](#collections) of the current asset.

To use the DialogueMaster, add the component to an object in your hierarchy and set the exposed fields in the Inspector.

Public methods in the DialogueMaster include:

#### ```Initialize()```

For initialization, you have the option to either initialize the master from the Awake method (by ticking the ```autoInitialize``` field) or by invoking this method manually. Proper initialization is required for the ```DialogueMaster``` to function correctly.

#### ```StartSituation(string situationName)```

This method allows you to explicitly start a situation at any moment. It will set the current node to either the **[StartNode](#start-node)** or **[FromSituationNode](#from-situation-node)**. No situation is set by default, so you need to call this method before using ```NextDialogueElement()```.

#### ```NextDialogueElement()```

Use this method to advance the dialogue flow in a given situation. It returns a **[DialogueNodeInfo](#dialoguenodeinfo)** that allows you to interact with the dialogue graph externally. Typically, you should use this method when a player makes a choice or wishes to progress in the dialogue.

Invoking ```NextDialogueElement()``` when **[DialogueNodeInfo](#dialoguenodeinfo)** returns ```IsChoiceNode``` before using ```SetSelectedChoice(int index)```, will result in the first choice being selected, previously selected choice selected, or an error if the number of choices in the current node is smaller than previously selected choice index.

#### ```SetSelectedChoice(int index)```

When arriving at the **[ChoiceNode](#choice-node)** **([DialogueNodeInfo](#dialoguenodeinfo)** returned with ```IsChoiceNode```), this method has to be called when the player makes their choice with the index of said choice. 

#### ```GetCollection<T>()```

This method allows you to get access to currently selected **[collection](#collections)** in the asset. Use the collection to interact with variables and events.

### **Entities**

You may access entities created in the editor through the ```DialogueMaster.EntitiesDatabase``` property.

### **Collections**

Collections are per asset generated scripts with variables and events defined in the graph's **[blackboard](#blackboard)**, and situation names defined in the **[tree view](#situations)**. They offer convenient read and write access to any variables created within the asset's EditorWindow tool.

#### **Automatic Generation**

Each collection is automatically generated within the **Runtime/Master/Collections** folder after creating a Dialogue Asset. It's worth noting that any modifications made to this file must be performed from within the asset, as external changes will not persist.

#### **Accessing Collections**

To access the current collection, use the ```DialogueMaster.GetCollection<T>()``` where T is the asset name followed by the word "Collection". Your code editor's IntelliSense should provide visibility to the collection after saving changes in the asset's EditorWindow.

#### **Runtime Creation**

Collections are dynamically created at runtime by the **[DialogueMaster](#dialoguemaster)** based on the asset you provide it with (field ```DialogueMaster.dialogueAsset```). 

#### **Managing Variables and Events**

All variable values within the collection can be both read and modified by the user. Additionally, users can subscribe to or unsubscribe from the events defined within the collection (refer to **[EventNode](#event-node)** for more details on events).

## **DialogueNodeInfo**

The **[DialogueNodeInfo](#dialoguenodeinfo)** is a crucial class utilized in the DialogueGraphBuilder. It's the main way to interact with the created graph. Use this class to display text, speakers and possible choices for the player.

To access **[DialogueNodeInfo](#dialoguenodeinfo)**, use ```DialogueMaster.NextDialogueElement()``` method provided by the **[DialogueMaster](#dialoguemaster)**. **[DialogueNodeInfo](#dialoguenodeinfo)** holds essential information about a dialogue node when traversing the conversation graph. It includes details such as the speaker, dialogue text, available choices, and whether the current situation has ended.

**[DialogueNodeInfo](#dialoguenodeinfo)** is exclusively sent when the dialogue flow is on [Dialogue](#dialogue-node), [Choice](#choice-node) and [End](#end-node) nodes. When sent, the dialogue flow pauses, allowing you to handle the current node yourself. You can resume the conversation flow by calling  ```DialogueMaster.NextDialogueElement()``` again when needed. All other nodes are processed internally immiedately, until the three above mentioned nodes are reached in the flow for you to use.

When **[ToSituationNode](#to-situation-node)** is encountered, it will immiedately switch to the next situation.

## **Graph View**

### **Blackboard**

The Blackboard serves as a repository for dialogue variables and events, much like ShaderGraph's blackboard.

![](Assets/Documentation/Images/Blackboard.png)

To create variables and events that can be utilized across **[situations](#situations)**, click the "+" icon located in the top right. The default value of each variable can be set by expanding the "Pill" with the ">" button. Event is a special Pill which doesn't have any default value, and can be dragged into the graph view to create an **[Event Node](#event-node)**.

### **Nodes**

Graph view supports a variety of nodes to help you build the flow. Additionally you can group nodes by right-clicking or navigating to **Dialogue>Group** in the search window, accessible via the Space key. Multiple selected nodes can be grouped simultaneously using these methods.

To remove a node from a group, hold Shift while clicking on the node. Except for the **[Start Node](#start-node)** any node can be deleted by either pressing the Del key on the keyboard or by right-clicking and selecting "Remove" on the contextual menu.

While DialogueGraphBuilder allows for cyclic node connections, exercise caution when creating such connections, as they may result in unintended behaviors.

#### **Dialogue Node**

Dialogue node is the simplest node in the project. It features an entity selection picker as well as a text field for entering dialogue.

#### **Choice Node**

Choice node allows you to set any number of choices presented to the player and redirect the conversation flow based on the selected choice. Clicking the "+" button will create a new choice and pressing "-" button will remove it. To specify the choice text given to the player, expand the choice foldout and enter the desired text.

When a Choice node is encountered at runtime, it returns a list of available choices in the **[DialogueNodeInfo](#dialoguenodeinfo)**.

#### **Expression Node**

The Expression Node allows to set a blackboard variable without touching any code.

This node allows for any number of expressions separated by ';', with support for basic C# operators such as =, *, +, -, and /.

Examples:

> MyInt = 3; MyInt = 3 * MyOtherInt; MyBool = MyOtherBool || MyThirdBool ...

#### **Condition Node**

Condition node is a powerfull node that allows to redirect the flow of dialogue based on variable conditions. 

Allowed expressions include constant number comparisons and comparisons of blackboard variables. Basic C# operators (<=, <, ==, >, >=, ||, &&) are supported.

Examples:

> MyInt <= 3;

> MyInt > MyOtherInt;

> 3 == 3;

> MyBool == true; 

For more details see [Expression Parser by Bunny83](https://github.com/Bunny83/LSystem)

#### **Start Node**

Start node is a fundamental node present in every **[Situation](#situations)**.

All the situations begin at the Start Node, unless the situation was switched to via **[To Situation Node](#to-situation-node)** or from code by using ```DialogueMaster.StartSituation(situationName)```.

#### **End Node**

The End Node marks the conclusion of a situation. Once the flow arrives at this node, ```DialogueMaster.NextDialogueElement()``` returns **[DialogueNodeInfo](#dialoguenodeinfo)** with SituationEnded set to true.

If it is attempted to move past the End Node with ```DialogueMaster.NextDialogueElement()```, **[DialogueNodeInfo](#dialoguenodeinfo)** will return the node the flow started this situation from. After the ```SituationEnded``` is sent, you may call ```DialogueMaster.StartSituation(situationName)``` to enter a different situation.

#### **From Situation Node**


This node enables you to specify the point from which the conversation should continue when transitioning from the previous situation to the current one. Within a situation, only one ```FromSituationNode``` can be connected to the same situation origin. These nodes allow for specific transition that would not be otherwise possible only by using Start Nodes.

When changing the situation by using **[To Situation Node](#to-situation-node)** or from code by using ```DialogueMaster.StartSituation(situationName)```, **[DialogueMaster](#dialoguemaster)** first checks if this node type is present with the value of the previous situation. If found, it continues the dialogue from that node. Otherwise it starts at the **[Start Node](#start-node)**.

In a situation, there can be no more than one From Situation Node with its value set to a given situation. All From Situation Nodes have to point to a different situation.

#### **To Situation Node**

This node allows you to change the current **[Situation](#situations)** to a specified one.

Unlike the **[From Situation Node](#from-situation-node)**, there can be multiple nodes of this type that navigate to the same situation.

When transitioning to the new situation using this node or via code with ```DialogueMaster.StartSituation(situationName)```, **[DialogueMaster](#dialoguemaster)** first checks for the presence of **[From Situation Node](#from-situation-node)** with the value of the situation in which this node is located. If such a node is found, it continues the dialogue from that **[From Situation Node](#from-situation-node)**, otherwise it starts at the the **[Start Node](#start-node)** in the new situation.

#### **Event Node**

Event nodes are created by dragging Event Pills into the **[Graph View](#graph-view)**. Once the flow reaches the event node, it triggers an event with the blackboard's event name in PascalCase, as defined in the **[collection](#collections)**.

# **Examples**

## **Princess and the King**

This example provides a straightforward demonstration of using the DialogueMaster's API and handling node flow. The scene includes a speaker portrait, a nameplate, and a text box, while the dialogue asset forms a linear connection through dialogue nodes.

```SimpleDialogueProgressor``` is a class that displays the StartDialogue button as well as makes the initial call to the **[DialogueMaster](#dialoguemaster)** prompting it to StartSituation by using the situation name provided by ```SimpleProgressionCollection```.

```csharp
...

public virtual void StartSituation()
{
    dialogueMaster.StartSituation(SimpleProgressionCollection.PrincessApproachesKing);
    ...
}
```

Once the StartDialogue button disappears from the screen, another invisible button activates with an onClick event to ProgressDialogue. ```SimpleDialogueProgressor``` then makes a call to **[DialogueMaster](#dialoguemaster)** to get the NextDialogueElement which returns **[DialogueNodeInfo](#dialoguenodeinfo)** for further handling.

This progressor primarily needs to know if the situation has ended or if the current node is a dialogue node. It sends its own events to notify any interested components of the current node. ```SimpleDialogueDisplayer``` listens to these events and uses **[DialogueNodeInfo](#dialoguenodeinfo)** to set the text presentation, speaker image and their name. 

```csharp
protected virtual DialogueNodeInfo ProgressInDialogue()
{
    DialogueNodeInfo nextNodeInfo = dialogueMaster.NextDialogueElement();

    if (nextNodeInfo == null)
    {
        return null;
    }

    if (nextNodeInfo.SituationEnded)
    {
        ...
        OnReceivedSituationEndedInfo?.Invoke(nextNodeInfo);
        // Situation ended
    }

    if (nextNodeInfo.IsDialogueNode)
    {
        OnReceivedDialogueInfo?.Invoke(nextNodeInfo);
        // Received node with text
    }

    return nextNodeInfo;
}
```

## **Guard at the gate**

This example demonstrates a slightly more complex scenario, highlighting the benefits of using node connections within the asset to fully leverage the modularity of situations. When arriving at the choice node, players are presented with a decision: attempt to bribe the guard to enter the city or choose to leave.

Once the choice is made to bribe the guard, an event ```OnBribeAtTheGate``` is sent for us to handle the result of attempted persuation. This is handled in ```PersuationChecker``` with a flip of the coin.

If the player succeeds in persuading the guard, the script asks the **[DialogueMaster](#dialoguemaster)** to start a situation where he is successful in his attempt, otherwise we sent the story to the route where he fails to enter the city. This then leads to a linear dialogue nodes connection which ends briefly.


```csharp
...

if (diceRoll > .5f)
{
    dialogueMaster.StartSituation(basicEventsCollection.GetSituationName(bribeAtTheGateSuccess));
    return;
}

dialogueMaster.StartSituation(basicEventsCollection.GetSituationName(bribeAtTheGateFailure));
```

In order for the ```PersuationChecker``` to get access to the situation name, it first needs the access to this story asset collection. To get the collection ```DialogueMaster.GetCollection<T>``` is used where T is the name of the asset in pascal case followed by the word "Collection".

## **Shopkeeper**

This example represents a more advanced scenario involving a shopkeeper. It features a tree of situations within the story editor, resembling a typical RPG game scenario.

In the situation, there is a StartDialogue button. Upon clicking this button, a full-screen button becomes active, enabling players to progress through the story.

The narrative begins with the ```ShopKeeperDialogueProgressor```, which makes the first call to the **[DialogueMaster](#dialoguemaster)** to access the asset collection and initiate the situation specified in the ```SituationNames``` enum found in the Collections. This enum is exposed with the [SerializeField] so that we can tweak it from the inspector.

```csharp
...

public override void StartSituation()
{
   ShopKeeperCollection collection = dialogueMaster.GetCollection<ShopKeeperCollection>();

   dialogueMaster.StartSituation(collection.GetSituationName(startSituation));
   ProgressInDialogue();
}
```

Calling ```dialogueMaster.StartSituation``` transitions the narrative to the ```StartNode```. The ```ProgressInDialogue``` method is used here, explained in **[Princess and the king](#princess-and-the-king)**, which makes a call to the **[DialogueMaster](#dialoguemaster)** to obtain the next node in the flow.

The key distinction between ```ShopKeeperDialogueProgressor``` and ```SimpleDialogueProgressor``` is the ability to send information when choice node is the current node. In other words, it's a simple extension to the ```SimpleDialogueProgressor``` that accounts for the choice nodes. When the ```DialogueNodeInfo.IsChoiceNode``` comes in as the next node, the following event is sent.

```csharp

DialogueNodeInfo nextNodeInfo = base.ProgressInDialogue();
...

if (nextNodeInfo.IsChoiceNode)
{
    OnReceivedChoiceInfo?.Invoke(nextNodeInfo);
    ...
}
```

As players advance through the narrative by pressing the screen button, the ```TwoSpeakerDialogueDisplayer``` receives events send by the ```ShopKeeperDialogueProgressor``` and displays appropriate information (entity images, names and the spoken text itself).

Throught the story the player gets the shopkeeper to introduce himself. After he does so, the boolean in the blackboard ```SimonIntroducedHimself``` is set to true in the expression node.

Once this happends, every time the shopkeeper speaks this variable is used to select the extra text list element from the **[Entities view](#entities)** to set his name to his real name.

This is achieved through the ```ShopKeeperNameProcessor``` whenever **[DialogueNodeInfo](#dialoguenodeinfo)** returns the ShopKeeper as a speaker. 

```csharp
public DialogueEntity Process(DialogueMaster master, DialogueEntity speaker)
{
    shopKeeperCollection ??= master.GetCollection<ShopKeeperCollection>();

    speaker.entityName = shopKeeperCollection.SimonIntroducedHimself 
       ? speaker.extraText[0] : speaker.entityName;

   return speaker;
}
```

By default, we set the shopkeeper's name in the **[EntityView](#entities)** to "Shop Keeper" (this is the value of ```speaker.entityName```), and his real name is set inside of ```speaker.extraText``` array.

The processor retrieves the collection from the **[DialogueMaster](#dialoguemaster)** and assigns the speaker's name based on whether SimonIntroducedHimself is true. This approach is safe because the speaker given as the parameter in the method is a clone of the original ScriptableObject.

A ```PlayerGold``` variable is exposed in the **[Blackboard](#blackboard)**. This variable is used in the dialogue to check if the player has enough gold to purchase a room. The variable is only used in the editor to get its value, while it's only set on the C# side by the ```MoneyManipulator```. 

When the player decides to buy the room, an event ```PlayerBoughtRoom``` is sent via the **[Collection](#collections)**.

in this example **[DialogueMaster](#dialoguemaster)** is set to initialize on Awake. To ensure proper event hook up ```MoneyManipulator``` subscribes to the appropriate event on both Start and OnEnable.

```csharp
private void OnEnable()
{
   if (!dialogueMaster.HasInitialized)
   {
       return;
   }

   shopKeeperCollection = dialogueMaster.GetCollection<ShopKeeperCollection>();
   shopKeeperCollection.PlayerBoughtRoom += ShopKeeperCollection_PlayerBoughtRoom; 
}

...

private void Start()
{
   shopKeeperCollection = dialogueMaster.GetCollection<ShopKeeperCollection>();
   shopKeeperCollection.PlayerBoughtRoom += ShopKeeperCollection_PlayerBoughtRoom;
}
```

Once the event fires, all that has to be done is to subtract the cost of the room from the ```PlayerGold``` variable in the Collection like from any other variable.

```csharp
...

private void ShopKeeperCollection_PlayerBoughtRoom()
{
   shopKeeperCollection.PlayerGold -= RoomCost;
}
```

# **Release Notes**

## **Version 1.0.0**

Initial release for Unity 2022.2.14

# **Additional Notes**

If you encounter any bugs, please feel free to create an issue on the GitHub page or submit a pull request if you've implemented a fix.

# **Credits**
- Bunny83, Expression Parser https://github.com/Bunny83/LSystem

### Additional credits for art assets used in screenshots
- CaptainCatSparrow, https://assetstore.unity.com/packages/2d/gui/icons/60-free-character-portraits-with-modular-background-242158
- Jesse Munguia, https://jesse-m.itch.io/jungle-pack
- Clembod, https://clembod.itch.io/warrior-free-animation-set
- Black Hammer, https://assetstore.unity.com/packages/2d/gui/fantasy-wooden-gui-free-103811
