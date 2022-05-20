using CitiesControllerMod.Helpers;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CitiesControllerMod.Services
{
    class TSBar
    {
        // Toolstrip
        public UIComponent Self; // TSBar
        public UIComponent MainToolstrip; // TSBar: components -> MainToolstrip
        public List<UIComponent> MainToolstripItemsSelectable; // TSBar: components -> MainToolstrip: components (that are selectable)
        public List<UIComponent> MainToolstripItems; // TSBar: components -> MainToolstrip: components
        public UIComponent ToolstripSelectionSprite;
    }

    class TSContainer
    {
        // Toolstrip tabs
        public UITabContainer Self; // TSContainer 
        public UITabContainer GTSContainer; // GTSContainer (group toolstrip)
        public List<UIComponent> GTSContainerScrollablePanelItems; // GTSContainer: items -> ScrollablePanel: items
        public GeneratedScrollPanel[] GeneratedScrollPanels; // grouptoolstrip needs this for navigation (should clarify what this does better)
        public UIComponent ToolbarTabItemSelectionSprite; // this panel object shows which gtscontainer item is selected
    }

    class TSTabGroup
    {
        public UITabstrip GroupToolstrip; // needed for navigating between the toolstrip tab 
    }

    class SpecialUIButtons
    {
        public UIButton Esc; // pause menu
        public UIButton TSClose; // for closing toolstrip with B
        public UIButton Freecamera;
        public UIMultiStateButton Bulldozer;
        public UIMultiStateButton Speed;
        public UIMultiStateButton Play;
    }

    class CursorTools
    {
        public NetTool[] NetTools;
    }

    public class UINavigationService
    {
        private FetchService FetchService = new FetchService();
        private MouseService MouseService = new MouseService();

        private TSBar tsBar = new TSBar();
        private TSContainer tsContainer = new TSContainer();
        private TSTabGroup tsTabGroup = new TSTabGroup();
        private SpecialUIButtons specialUIButtons = new SpecialUIButtons();
        private CursorTools cursorTools = new CursorTools();


        UIView AView;

        private int toolstripBarHoverIndex = -1;
        private bool toolstripTabIsOpen = false;
        private bool toolstripIsInInspectMode = false;

        // states & updates
        public bool ToolstripTabIsOpen { get => toolstripTabIsOpen; }
        public bool ToolstripIsInInspectMode { get => toolstripIsInInspectMode; }
        public void UpdateToolstripStates()
        {
            toolstripTabIsOpen = tsContainer.Self.selectedIndex != -1;
            toolstripIsInInspectMode = toolstripBarHoverIndex == -1;
            try
            {
                if (tsBar.ToolstripSelectionSprite != null && !toolstripIsInInspectMode)
                {
                    UIComponent label = tsBar.ToolstripSelectionSprite.GetComponent<UIComponent>();
                    label.isVisible = !toolstripTabIsOpen;
                }
            }
            catch (Exception)
            { }
        }
        public void UpdateGTSContainer()
        {
            tsContainer.GTSContainer = FetchService.FetchGTSContainer(tsContainer.Self);
        }

        // toolstrip bar navigation
        public string ToolstripSelectionName()
        {
            return tsBar.MainToolstripItems[tsContainer.Self.selectedIndex].name;
        }
        public void ToolstripSetSelectedItemHover(bool apply)
        {
            try
            {
                UIButton toolstripItemToSelect = UIView.Find<UIButton>(tsBar.MainToolstripItems[toolstripBarHoverIndex].name);
                toolstripItemToSelect.state = apply ? UIButton.ButtonState.Hovered : UIButton.ButtonState.Normal;
            }
            catch (Exception) { }
        }
        public void ToolstripMoveSelection(bool isNext)
        {
            // at low framerates there is a small lag when this runs. needs fix
            // selectedIndex = -1 means "none chosen". could be useful for implementing console-style nav here
            try
            {
                UIComponent component;
                int offset = 0;
                if (toolstripBarHoverIndex == -1)
                {
                    toolstripBarHoverIndex = 0;
                }
                else
                {
                    offset = isNext ? +1 : -1;
                }

                var selectedComponent = tsBar.MainToolstripItems[toolstripBarHoverIndex];
                component = tsBar.MainToolstripItemsSelectable[tsBar.MainToolstripItemsSelectable.FindIndex(x => x.name == selectedComponent.name) + offset];

                toolstripBarHoverIndex = component.zOrder;
                tsBar.ToolstripSelectionSprite.position = component.position;
                tsBar.ToolstripSelectionSprite.relativePosition = component.relativePosition;
                tsBar.ToolstripSelectionSprite.absolutePosition = component.absolutePosition;
                tsBar.ToolstripSelectionSprite.size = component.size;
                tsBar.ToolstripSelectionSprite.isVisible = true;
                var panel = tsBar.ToolstripSelectionSprite.components[0] as UILabel;
                panel.text = component.name;
            }
            catch (Exception)
            { }
        }
        public void ToolstripClickSelection()
        {
            try
            {
                tsBar.MainToolstripItems[toolstripBarHoverIndex].SimulateClick();
                MouseService.MoveMouseToScreenCenter();
                UpdateGTSContainer();
            }
            catch (Exception) { }
        }
        public void GoToInspectMode()
        {
            try
            {
                tsBar.ToolstripSelectionSprite.isVisible = false;
            }
            catch (Exception) { }

            tsContainer.Self.selectedIndex = -1;
            toolstripBarHoverIndex = -1;
        }

        // toolstrip tab navigation
        public GeneratedScrollPanel SelectedTabItemScrollPanel()
        {
            try
            {
                if (tsContainer.GeneratedScrollPanels.Count() > 0) // this is what needs UpdateGTSContainer
                {
                    return tsContainer.GeneratedScrollPanels.Where(x => x.name == tsContainer.GTSContainer.components[tsContainer.GTSContainer.selectedIndex].name).FirstOrDefault();
                }
            }
            catch (Exception)
            { }
            return null;
        }
        public void ToolstripTabMoveSelection(bool isNext)
        {
            try
            {
                var currentScrollPanel = SelectedTabItemScrollPanel();
                if (currentScrollPanel != null)
                {
                    //if decrement::: theres an issue with sleectedindex -1 here. shouldnt be able to non-select with dpad in gts.
                    if (currentScrollPanel.selectedIndex == -1)
                        currentScrollPanel.selectedIndex = 0;

                    var navigatingIntoNegatives = !isNext && currentScrollPanel.selectedIndex == 0;

                    if (!navigatingIntoNegatives)
                    {
                        currentScrollPanel.selectedIndex = isNext ? currentScrollPanel.selectedIndex + 1 : currentScrollPanel.selectedIndex - 1;
                    }
                    foreach (var item in tsContainer.GTSContainer.components)
                    {
                        if (item.name == currentScrollPanel.name)
                        {
                            UIScrollablePanel scrollablepanel = new UIScrollablePanel();
                            UIScrollbar scrollbar = new UIScrollbar();

                            foreach (var item2 in item.components)
                            {
                                if (item2.name == "ScrollablePanel")
                                {
                                    scrollablepanel = item2 as UIScrollablePanel;
                                }
                                if (item2.name == "Scrollbar")
                                {
                                    scrollbar = item2 as UIScrollbar;
                                }
                            }

                            bool doscroll = false;

                            if (scrollablepanel.name != null)
                            {
                                UIButton result = scrollablepanel.components[currentScrollPanel.selectedIndex] as UIButton;
                                result.SimulateClick(); // next up: disable tool when this hits a disabled item. keep tool enabled otherwise.
                                if (result.position.x < 0 || result.position.x > 700)
                                {
                                    doscroll = true;
                                }
                            }

                            if (scrollbar.name != null)
                            {
                                if (doscroll)
                                {
                                    typeof(UIScrollbar)
                                        .GetMethod("ScrollEase", BindingFlags.NonPublic | BindingFlags.Instance)
                                        .Invoke(scrollbar, new object[1] { isNext ? scrollbar.incrementAmount : -scrollbar.incrementAmount });
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }

        public void RenderToolStripTabHover()
        {
            try
            {
                var selectedTabItemScrollPanel = SelectedTabItemScrollPanel();
                if (selectedTabItemScrollPanel != null)
                {
                    foreach (var gtsContainerComponent in this.tsContainer.GTSContainer.components)
                    {
                        if (gtsContainerComponent.name == selectedTabItemScrollPanel.name)
                        {
                            foreach (var gtsContainerDeeperItem in gtsContainerComponent.components) // needs a better name.
                            {
                                if (gtsContainerDeeperItem.name == "ScrollablePanel")
                                {
                                    var toolstripSelectionName = ToolstripSelectionName();
                                    var visible = toolstripSelectionName != "Money" && toolstripSelectionName != "Policies";
                                    var result = gtsContainerDeeperItem.components[selectedTabItemScrollPanel.selectedIndex];
                                    tsContainer.ToolbarTabItemSelectionSprite.position = result.position;
                                    tsContainer.ToolbarTabItemSelectionSprite.relativePosition = result.relativePosition;
                                    tsContainer.ToolbarTabItemSelectionSprite.absolutePosition = result.absolutePosition;
                                    tsContainer.ToolbarTabItemSelectionSprite.size = result.size;
                                    tsContainer.ToolbarTabItemSelectionSprite.isVisible = ToolstripTabIsOpen && visible;
                                    var label = tsContainer.ToolbarTabItemSelectionSprite.components[0] as UILabel;
                                    label.text = result.name;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception)
            { }
        }

        // toolstrip tab group navigation
        public void ToolstripTabGroupMoveSelection(bool isNext)
        {
            try
            {
                var navigatingIntoNegatives = !isNext && tsTabGroup.GroupToolstrip.selectedIndex == 0;
                if (isNext)
                {
                    tsTabGroup.GroupToolstrip.selectedIndex++;
                }
                else if (!navigatingIntoNegatives)
                {
                    tsTabGroup.GroupToolstrip.selectedIndex--;
                }
            }
            catch (Exception)
            { }
        }

        // net tool
        public int NetToolControlPointCount()
        {
            int controlPointCount = 0;
            try
            {
                var netTool = cursorTools.NetTools[0];
                controlPointCount = netTool.GetFieldValue<int>("m_controlPointCount");
            }
            catch (Exception)
            { }
            return controlPointCount;
        }

        public void SetNetToolEnabled(bool enabled)
        {
            try
            {
                var netTool = cursorTools.NetTools[0];
                netTool.enabled = enabled;
            }
            catch (Exception)
            { }
        }

        //button actions
        public void PressEscButton()
        {
            specialUIButtons.Esc.SimulateClick();
        }
        public void PressTSCloseButton()
        {
            specialUIButtons.TSClose.SimulateClick();
            tsContainer.ToolbarTabItemSelectionSprite.isVisible = false;
        }
        public void PressFreecameraButton()
        {
            specialUIButtons.Freecamera.SimulateClick();
        }
        public void PressBulldozerButton()
        {
            specialUIButtons.Bulldozer.SimulateClick();
        }
        
        public void PressSpeedButton()
        {
            specialUIButtons.Speed.SimulateClick();
        }
        public void PressPlayButton()
        {
            specialUIButtons.Play.SimulateClick();
        }

        // bulk fetching
        public void EnsureUIElementAvailability()
        {
            // aview
            if (AView == null)
                AView = UIView.GetAView();

            // toolstrip fetch
            if (tsBar.Self == null)
                tsBar.Self = FetchService.FetchTSBar();
            if (tsBar.MainToolstrip == null)
                tsBar.MainToolstrip = FetchService.FetchTSBarMainToolStrip();
            if (tsBar.MainToolstripItems == null)
                tsBar.MainToolstripItems = FetchService.FetchTSBarMainStoolStripItemsAll(tsBar.MainToolstrip);
            if (tsBar.MainToolstripItemsSelectable == null)
                tsBar.MainToolstripItemsSelectable = FetchService.FetchTSBarMainToolStripItemsSelectable(tsBar.MainToolstripItems);

            // toolstrip tabs fetch
            if (tsContainer.Self == null)
                tsContainer.Self = FetchService.FetchTSTabContainer();
            if (tsContainer.GTSContainer == null)
                tsContainer.GTSContainer = FetchService.FetchGTSContainer(tsContainer.Self);
            if (tsContainer.GTSContainerScrollablePanelItems == null)
                tsContainer.GTSContainerScrollablePanelItems = FetchService.FetchTSPanelScrollablePanelItems(tsContainer.GTSContainer);
            if (tsContainer.GeneratedScrollPanels == null)
                tsContainer.GeneratedScrollPanels = FetchService.FetchGeneratedScrollPanels();

            // toolstrip tab groups fetch
            if (tsTabGroup.GroupToolstrip == null)
                tsTabGroup.GroupToolstrip = FetchService.FetchGroupToolstrip();

            // special buttons fetch
            if (specialUIButtons.Esc == null)
                specialUIButtons.Esc = FetchService.FetchEscButton();
            if (specialUIButtons.TSClose == null)
                specialUIButtons.TSClose = FetchService.FetchTSCloseButton();
            if (specialUIButtons.Freecamera == null)
                specialUIButtons.Freecamera = FetchService.FetchFreecameraButton();
            if (specialUIButtons.Bulldozer == null)
                specialUIButtons.Bulldozer = FetchService.FetchBulldozerButton();
            if (specialUIButtons.Speed == null)
                specialUIButtons.Speed = FetchService.FetchSpeedButton();
            if (specialUIButtons.Play == null)
                specialUIButtons.Play = FetchService.FetchPlayButton();

            // fetch cursor tools
            if (cursorTools.NetTools == null)
                cursorTools.NetTools = FetchService.FetchNetTools();

            LoadCustomUIElements();
        }

        private void LoadCustomUIElements()
        {
            try
            {
                tsBar.ToolstripSelectionSprite = AView.FindUIComponent(new ToolstripSelectionSprite().name);
                tsContainer.ToolbarTabItemSelectionSprite = AView.FindUIComponent(new ToolbarTabItemSelectionSprite().name);
            }
            catch (Exception)
            { }

            if (tsBar.ToolstripSelectionSprite == null)
                tsBar.ToolstripSelectionSprite = AView.AddUIComponent(typeof(ToolstripSelectionSprite));

            if (tsContainer.ToolbarTabItemSelectionSprite == null)
                tsContainer.ToolbarTabItemSelectionSprite = AView.AddUIComponent(typeof(ToolbarTabItemSelectionSprite));
        }
    }

    public class ToolbarTabItemSelectionSprite : UIPanel
    {
        public override void Start()
        {
            this.backgroundSprite = "UnlockingPanel2";
            this.color = new Color32(200, 200, 255, 100);
            this.width = 0;
            this.height = 0;
            this.position = new Vector2(0, 0);
            this.isVisible = false;
            this.name = "ToolbarTabItemSelectionSprite";
            UILabel l = this.AddUIComponent<UILabel>();
            l.text = "";
            l.backgroundSprite = "ScrollbarTrack";
            l.font.size = 16;
            l.relativePosition = new Vector2(0, -50);
        }
    }

    public class ToolstripSelectionSprite : UIPanel
    {
        public override void Start()
        {
            this.backgroundSprite = "ListItemHover";
            this.color = new Color32(200, 200, 255, 100);
            this.width = 0;
            this.height = 0;
            this.position = new Vector2(0, 0);
            this.name = "ToolstripSelectionSprite";
            UILabel l = this.AddUIComponent<UILabel>();
            l.text = "";
            l.font.size = 20;
            l.relativePosition = new Vector2(0, -20);
        }
    }
}
