using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesControllerMod.UIComponents
{
    public class RadialMenuMainComponent : UIComponent
    {
        public override void Start()
        {
            color = new Color32(255, 255, 255, 245);
            size = new Vector2(0, 0);
            position = new Vector2(0, 0);
            isVisible = true;
            name = "RadialMenuMainComponent";
            enabled = false;
        }
    }
}
