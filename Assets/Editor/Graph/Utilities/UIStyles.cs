using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public static class UIStyles
    {
        public const string TextFieldHiddenUSS = "base-node__textfield__hidden";

        public static Color ErrorColor { get; private set; } = new Color(255f / 255f, 0f / 255f, 0f / 255f);
        public static Color PropertyColor { get; private set; } = new Color(118f / 255f, 82f / 255f, 140f / 255f);
        public static Color LogicColor { get; private set; } = new Color(44f / 255f, 110f / 255f, 173f / 255f);
        public static Color EventColor { get; private set; } = new Color(82f / 255f, 140f / 255f, 82f / 255f);
        public static Color StoryColor { get; private set; } = new Color(132f / 255f, 54f / 255f, 58f / 255f);
        public static Color StoryLighterColor { get; private set; } =
            new Color((StoryColor.r * 255f + 7f) / 255f, (StoryColor.g * 255f + 7f) / 255f, (StoryColor.b * 255f + 7f) / 255f);
        public static Color StoryDarkerColor { get; private set; } = 
            new Color((StoryColor.r * 255f - 11f) / 255f, (StoryColor.g * 255f - 11f) / 255f, (StoryColor.b * 255f - 11f) / 255f);
        public static Color TransferColor { get; private set; } = new Color(22f / 255f, 22f / 255f, 22f / 255f);
        public static Color DefaultDarkColor { get; private set; } = new Color(59f / 255f, 59f / 255f, 59f / 255f);
        public static Color DefaultDarker0Color { get; private set; } = new Color(45f / 255f, 45f / 255f, 45f / 255f);
        public static Color DefaultDarkerColor { get; private set; } = new Color(40f / 255f, 40f / 255f, 40f / 255f);
        public static Color DefaultDarker2Color { get; private set; } = new Color(36f / 255f, 36f / 255f, 36f / 255f);
        public static Color DefaultLightColor { get; private set; } = new Color(180f / 255f, 180f / 255f, 180f / 255f);

        public const float FontSize = 14f;
        public const float FontSizeBig = 20f;

        public const float LogicMarginTop = 10f;
        public const float PaddingSmall = 4f;
        public const float PaddingMedium = 8f;

        public const float MaxWidth = 250f;
        public const float MaxHeight = 150f;
        public const float SmallTextFieldHeight = 50f;

        public const float ConstantPortInputMinWidth = 104f;
        public const float ConstantPortInputMinHeight = 22f;
        public const float ConstantPortInputMarginLeft = -110f;
        public const float ConstantPortWidth = 8f;
        public const float ConstantPortOffset = 4f;

        public const float EntityHeaderNameRadius = 3f;
        public const float EntityHeaderNameRadiusSmall = .03f;
        public const float EntityHeaderNameRadiusTiny = .01f;

        public const float ListViewItemHeight = 30f;
    }
}