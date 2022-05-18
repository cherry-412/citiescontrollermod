using ColossalFramework.UI;
using static CitiesControllerMod.Helpers.MouseOperations;

namespace CitesControllerMod.Helpers
{
    public class MathHelper
    {
        public static MousePoint FindCenterOfUIComponent(UIComponent component)
        {
            MousePoint result = new MousePoint();
            result.X = (int)component.absolutePosition.x + (int)(component.width / 2);
            result.Y = (int)component.absolutePosition.y + (int)(component.height / 2);
            return result;
        }
    }
}
