using ColossalFramework.UI;
using UnityEngine;

namespace CitiesControllerMod.UIComponents
{
    public class FakeNetTool : UIPanel
    {
        public override void Start()
        {
            this.backgroundSprite = "DistrictOptionBrushLargeFocused";
            this.color = new Color32(255, 255, 255, 65);
            this.size = new Vector2(14, 14);
            this.position = new Vector2(-7, 7);
            this.isVisible = true;
            this.name = "FakeToolCursor";
            this.canFocus = false;
            this.isInteractive = false;
        }
    }
}
