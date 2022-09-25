using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesControllerMod.UIComponents
{
    public class RadialMenuSelectionComponent : UIPanel
    {
        public static Color32 SelectedColor = new Color32(255, 255, 255, 255);
        public static Color32 DefaultColor = new Color32(200, 200, 255, 255);
        public override void Start()
        {
            backgroundSprite = "ButtonMenu";
            size = new Vector2(200, 200);
            position = new Vector2(0, +100);
            isVisible = true;
            name = "RadialMenuSelectionComponent";
            enabled = false;
            color = DefaultColor;

            UILabel l = AddUIComponent<UILabel>();
            l.name = "SelectionText";
            l.text = "unset";
            l.position = new Vector2(0, -size.y / 2 + 25);
            l.textAlignment = UIHorizontalAlignment.Center;
            l.verticalAlignment = UIVerticalAlignment.Middle;
            l.autoSize = false;
            l.size = new Vector2(size.x, size.y);

            UIPanel icon = AddUIComponent<UIPanel>();
            l.name = "SelectionIcon";
            icon.backgroundSprite = "ToolbarIconHelp";
            icon.size = new Vector2(size.x / 2, size.y / 2);
            int verticalOffset = 10;
            icon.position = new Vector2(size.x / 2 - icon.size.x / 2, -icon.size.y / 2 + verticalOffset);
        }
    }
}
