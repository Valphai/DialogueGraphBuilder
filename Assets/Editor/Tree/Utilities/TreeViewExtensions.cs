namespace UnityEngine.UIElements
{
    public static class TreeViewExtensions
    {
        public static int GetDepthOfItemById(this TreeView treeView, int id)
        {
            int depth = 0;

            int parentId = treeView.viewController.GetParentId(id);
            while (parentId != -1)
            {
                parentId = treeView.viewController.GetParentId(parentId);

                depth++;
            }

            return depth;
        }
    }
}