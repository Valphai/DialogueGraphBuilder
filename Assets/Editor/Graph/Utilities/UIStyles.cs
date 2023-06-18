using UnityEngine;

namespace Chocolate4.Edit.Graph.Utilities
{
    public static class UIStyles
    {
        public const string TextFieldHiddenUSS = "base-node__textfield__hidden";

        public static Color PropertyColor { get; private set; } = new Color(118f / 255f, 82f / 255f, 140f / 255f);
        public static Color LogicColor { get; private set; } = new Color(82f / 255f, 140f / 255f, 82f / 255f);
        public static Color StoryColor { get; private set; } = new Color(132f / 255f, 54f / 255f, 58f / 255f);
        public static Color TransferColor { get; private set; } = new Color(22f / 255f, 22f / 255f, 22f / 255f);
        public static Color DefaultDarkColor { get; private set; } = new Color(59f / 255f, 59f / 255f, 59f / 255f);
        public static Color DefaultDarkerColor { get; private set; } = new Color(33f / 255f, 35f / 255f, 37f / 255f);
        public static Color DefaultLightColor { get; private set; } = new Color(180f / 255f, 180f / 255f, 180f / 255f);

        public const float LogicFontSize = 14f;

        public const float LogicMarginTop = 10f;
        public const float PaddingSmall = 4f;
        public const float PaddingMedium = 8f;

        public const float MaxWidth = 250f;
        public const float MaxHeight = 150f;

        public const float ConstantPortInputMinWidth = 104f;
        public const float ConstantPortInputMinHeight = 22f;
        public const float ConstantPortInputMarginLeft = -110f;
        public const float ConstantPortWidth = 8f;
        public const float ConstantPortOffset = 4f;
    }
}