using ColossalFramework.UI;
using UnityEngine;

namespace CitiesControllerMod.UIComponents
{
    public class InspectionModeNotifier : UILabel
    {
        public override void Start()
        {
            this.backgroundSprite = "ScrollbarTrack";
            name = "InspectionModeNotifier";
            text = "Inspect Mode ON";
            absolutePosition = new Vector2(388, 999);
            textAlignment = UIHorizontalAlignment.Center;
            verticalAlignment = UIVerticalAlignment.Middle;
            autoSize = false;
            size = new Vector2(175, 28);
            isVisible = false;
        }
    }
}
