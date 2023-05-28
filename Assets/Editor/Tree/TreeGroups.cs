namespace Chocolate4.Tree
{
    public enum TreeGroups
    {
        Situation,
        Variable,
        Event
    }

    public enum TreeItemType
    {
        Group,
        Item
    }

    public static class TreeGroupsExtensions
    {
        public const string SituationString = "S";
        public const string VariableString = "V";
        public const string EventString = "E";
        public const string VariableGroupString = "VG";
        public const string EventGroupString = "EG";

        public const string DefaultSituationName = "New Situation";
        public const string DefaultVariableGroupName = "New Variable Group";
        public const string DefaultEventGroupName = "New Event Group";
        
        public const string DefaultVariableName = "New Variable";
        public const string DefaultEventName = "New Event";

        public static string GetString(this TreeGroups treeGroup, TreeItemType itemType) => treeGroup switch
        {
            TreeGroups.Situation => SituationString,
            TreeGroups.Variable => itemType == TreeItemType.Group ? VariableGroupString : VariableString,
            TreeGroups.Event => itemType == TreeItemType.Group ? EventGroupString : EventString,
            _ => string.Empty
        };
    }
}
