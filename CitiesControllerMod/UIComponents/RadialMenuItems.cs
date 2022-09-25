using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CitiesControllerMod.UIComponents
{
    static class RadialMenuItems
    {
        public static List<RadialMenuItem> DefaultRadialMenu = new List<RadialMenuItem>(){
            new RadialMenuItem("Economy", "ToolbarIconMoney", "PressEconomyButton"),
            new RadialMenuItem("Areas", "ToolbarIconZoomOutGlobe", "PressAreasButton"),
            new RadialMenuItem("City Info", "CityInfo", "PressCityInfoButton"),
            new RadialMenuItem("Policies", "ToolbarIconPolicies", "PressPoliciesButton"),
            new RadialMenuItem("Milestones", "LockIcon", "PressMilestonesButton"),
            new RadialMenuItem("Info Views", "InfoPanelIconInfo", "PressInfoViewsButton")};
    }
}
