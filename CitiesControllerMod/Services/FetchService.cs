using ColossalFramework.UI;
using System;
using System.Collections.Generic;

namespace CitiesControllerMod.Services
{
    public class FetchService
    {
        // UI: Toolstrip
        public UIComponent FetchTSBar()
        {
            return UIView.Find("TSBar");
        }
        public UIComponent FetchTSBarMainToolStrip()
        {
            return UIView.Find<UIComponent>("MainToolstrip");
        }
        public List<UIComponent> FetchTSBarMainStoolStripItemsAll(UIComponent mainToolstrip)
        {
            List<UIComponent> result = new List<UIComponent>();
            foreach (var item in mainToolstrip.components)
            {
                result.Add(item);
            }
            return result;
        }
        public List<UIComponent> FetchTSBarMainToolStripItemsSelectable(List<UIComponent> mainToolstripItems)
        {
            List<UIComponent> result = new List<UIComponent>();
            result.AddRange(mainToolstripItems);
            result.RemoveAll(x => x.name == "Separator" || x.name == "SmallSeparator" || !x.isEnabled);
            return result;
        }

        // UI: Toolstrip tabs
        public UITabContainer FetchTSTabContainer()
        {
            return UIView.Find<UITabContainer>("TSContainer");
        }
        public UITabContainer FetchGTSContainer(UITabContainer tscontainer)
        {
            if (tscontainer.selectedIndex != -1)
            {
                foreach (var component in tscontainer.components[tscontainer.selectedIndex].components)
                {
                    if (component.name == "GTSContainer")
                    {
                        return component as UITabContainer;
                    }
                }
            }
            return UIView.Find<UITabContainer>("GTSContainer"); // otherwise it'll null. maybe look into this one
        }
        public List<UIComponent> FetchTSPanelScrollablePanelItems(UITabContainer gtsContainer)
        {
            List<UIComponent> gtsContainerItems = new List<UIComponent>();
            foreach (var component in gtsContainer.components)
            {
                gtsContainerItems.Add(component);
            }

            List<UIComponent> result = new List<UIComponent>();
            if (gtsContainerItems.Count > 0)
            {
                foreach (var containerItem in gtsContainerItems[0].components)
                {
                    if (containerItem.name == "ScrollablePanel")
                    {
                        foreach (var item in containerItem.components)
                        {
                            result.Add(item);
                        }
                    }
                }
            }
            return result;
        }
        public GeneratedScrollPanel[] FetchGeneratedScrollPanels()
        {
            try
            {
                return UnityEngine.Object.FindObjectsOfType<GeneratedScrollPanel>();
            }
            catch (Exception)
            { }
            return null;
        }

        // UI: Toolstrip tab groups
        public UITabstrip FetchGroupToolstrip()
        {
            return UIView.Find<UITabstrip>("GroupToolstrip");
        }


        // UI: Special buttons
        public UIButton FetchEscButton()
        {
            return UIView.Find<UIButton>("Esc");
        }
        public UIButton FetchTSCloseButton()
        {
            return UIView.Find<UIButton>("TSCloseButton");
        }
        public UIButton FetchFreecameraButton()
        {
            return UIView.Find<UIButton>("Freecamera");
        }
        public UIMultiStateButton FetchBulldozerButton()
        {
            return UIView.Find<UIMultiStateButton>("BulldozerButton");
        }

        // Cursor tools
        public NetTool[] FetchNetTools()
        {
            try
            {
                return UnityEngine.Object.FindObjectsOfType<NetTool>();
            }
            catch (Exception)
            { }
            return null;
        }
    }
}
