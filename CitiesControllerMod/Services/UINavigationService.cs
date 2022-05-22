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
        private CameraService CameraService = new CameraService();

        UIView AView;
        private TSBar tsBar = new TSBar();
        private TSContainer tsContainer = new TSContainer();
        private TSTabGroup tsTabGroup = new TSTabGroup();
        private SpecialUIButtons specialUIButtons = new SpecialUIButtons();
        private CursorTools cursorTools = new CursorTools();

        public static int ToolstripBarHoverIndex = -1;
        private bool toolstripTabIsOpen = false;
        private bool toolstripIsInInspectMode = false;

        // states & updates
        public bool ToolstripTabIsOpen { get => toolstripTabIsOpen; }
        public bool ToolstripIsInInspectMode { get => toolstripIsInInspectMode; }
        public void UpdateToolstripStates()
        {
            toolstripTabIsOpen = tsContainer.Self.selectedIndex != -1;
            toolstripIsInInspectMode = ToolstripBarHoverIndex == -1;
            if (tsBar.ToolstripSelectionSprite != null && !toolstripIsInInspectMode)
            {
                UIComponent label = tsBar.ToolstripSelectionSprite.GetComponent<UIComponent>();
                label.isVisible = !toolstripTabIsOpen;
            }
            HideCustomUIComponentOnMouseHover(tsBar.ToolstripSelectionSprite);
        }
        public static void ResetToolstripBarIndexes()
        {
            try
            {
                var tscontainer = UIView.Find<UITabContainer>("TSContainer");
                tscontainer.selectedIndex = -1;
                ToolstripBarHoverIndex = -1;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        public void UpdateGTSContainer()
        {
            tsContainer.GTSContainer = FetchService.FetchGTSContainer(tsContainer.Self);
        }

        public void UpdateGroupToolstrip()
        {
            tsTabGroup.GroupToolstrip = tsContainer.Self.selectedIndex != -1 ? FetchService.FetchGroupToolstrip(tsContainer.Self) : null;
        }

        // toolstrip bar navigation
        public string ToolstripSelectionName()
        {
            UpdateToolstripStates();
            string result = "";
            try
            {
                if (toolstripTabIsOpen)
                {
                    result = tsBar.MainToolstripItems[tsContainer.Self.selectedIndex].name;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return result;
        }

        public void ToolstripMoveSelection(bool isNext)
        {
            try
            {
                UIComponent component;
                int offset = 0;
                if (ToolstripBarHoverIndex == -1)
                {
                    ToolstripBarHoverIndex = 0;
                }
                else
                {
                    offset = isNext ? +1 : -1;
                }
                var selectedComponent = tsBar.MainToolstripItems[ToolstripBarHoverIndex];
                var newSelectionIndex = tsBar.MainToolstripItemsSelectable.FindIndex(x => x.name == selectedComponent.name) + offset;
                if (isNext && newSelectionIndex > tsBar.MainToolstripItemsSelectable.Count - 1)
                {
                    newSelectionIndex = 0;
                }
                else if (!isNext && newSelectionIndex < 0)
                {
                    newSelectionIndex = tsBar.MainToolstripItemsSelectable.Count - 1;
                }
                component = tsBar.MainToolstripItemsSelectable[newSelectionIndex];
                ToolstripBarHoverIndex = component.zOrder;
                tsBar.ToolstripSelectionSprite.position = component.position;
                tsBar.ToolstripSelectionSprite.relativePosition = component.relativePosition;
                tsBar.ToolstripSelectionSprite.absolutePosition = component.absolutePosition;
                tsBar.ToolstripSelectionSprite.size = component.size;
                tsBar.ToolstripSelectionSprite.isVisible = true;
                var panel = tsBar.ToolstripSelectionSprite.components[0] as UILabel;
                panel.text = component.name;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        public void ToolstripClickSelection()
        {
            tsBar.MainToolstripItems[ToolstripBarHoverIndex].SimulateClick();
            MouseService.MoveMouseToScreenCenter();
            UpdateGTSContainer();
            UpdateGroupToolstrip();
        }
        public void GoToInspectMode()
        {
            try
            {
                tsBar.ToolstripSelectionSprite.isVisible = false;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            tsContainer.Self.selectedIndex = -1;
            ToolstripBarHoverIndex = -1;
        }

        // toolstrip tab navigation
        public GeneratedScrollPanel SelectedTabItemScrollPanel()
        {
            try
            {
                if (tsContainer.GeneratedScrollPanels.Count() > 0 && tsContainer.GTSContainer.selectedIndex >= 0) // this is what needs UpdateGTSContainer
                {
                    return tsContainer.GeneratedScrollPanels.Where(x => x.name == tsContainer.GTSContainer.components[tsContainer.GTSContainer.selectedIndex].name).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return null;
        }
        public void ToolstripTabMoveSelection(bool isNext)
        {
            var currentScrollPanel = SelectedTabItemScrollPanel();
            if (currentScrollPanel != null)
            {
                if (currentScrollPanel.selectedIndex == -1)
                    currentScrollPanel.selectedIndex = 0;

                int offset = isNext ? 1 : -1;
                int newSelectionIndex = currentScrollPanel.selectedIndex + offset;
                int max = currentScrollPanel.childComponents.Count;
                bool scrollWrapAround = false;

                if (isNext && newSelectionIndex > max - 1)
                {
                    newSelectionIndex = 0;
                    scrollWrapAround = true;
                }
                else if (!isNext && newSelectionIndex < 0)
                {
                    newSelectionIndex = max - 1;
                    scrollWrapAround = true;
                }
                currentScrollPanel.selectedIndex = newSelectionIndex;
                //scrolling new item into view
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
                                var incrementAmount = scrollbar.incrementAmount;
                                if (scrollWrapAround)
                                {
                                    incrementAmount = isNext ? -50000 : 50000;
                                }
                                else
                                {
                                    incrementAmount = isNext ? incrementAmount : -incrementAmount;
                                }
                                typeof(UIScrollbar)
                                    .GetMethod("ScrollEase", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Invoke(scrollbar, new object[1] { incrementAmount });
                            }
                        }
                    }
                }
            }
        }

        public void RenderToolStripTabHover()
        {
            try
            {
                var selectedTabItemScrollPanel = SelectedTabItemScrollPanel();
                if (selectedTabItemScrollPanel != null)
                {
                    if (selectedTabItemScrollPanel.selectedIndex == -1)
                        selectedTabItemScrollPanel.selectedIndex = 0;

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
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        // toolstrip tab group navigation
        public void ToolstripTabGroupMoveSelection(bool isNext)
        {
            var thereAreTabsToNavigate = tsContainer.GTSContainer.components.Count >= 2;
            if (thereAreTabsToNavigate)
            {
                try
                {
                    var offset = isNext ? 1 : -1;
                    var newSelectionIndex = tsContainer.GTSContainer.selectedIndex + offset;
                    var max = tsContainer.GTSContainer.components.Count;
                    if (isNext && newSelectionIndex > max - 1)
                    {
                        newSelectionIndex = 0;
                    }
                    else if (!isNext && newSelectionIndex < 0)
                    {
                        newSelectionIndex = max - 1;
                    }
                    tsContainer.GTSContainer.selectedIndex = newSelectionIndex;
                    tsTabGroup.GroupToolstrip.selectedIndex = newSelectionIndex;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
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
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return controlPointCount;
        }

        public void SetNetToolEnabled(bool enabled)
        {
            try
            {
                var netTool = cursorTools.NetTools[0];
                netTool.enabled = enabled;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
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
            var cameraController = CameraService.MainCameraController;
            if (cameraController != null)
            {
                if (cameraController.m_freeCamera)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    MouseService.MoveMouseToScreenCenter();
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        public void PressBulldozerButton()
        {
            specialUIButtons.Bulldozer.SimulateClick();
            MouseService.MoveMouseToScreenCenter();
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
        public void EnsureUIElementAvailability(bool force)
        {

            if (force)
                LoadingActions.UIReloadNeeded = false;

            // aview
            if (force || AView == null)
                AView = UIView.GetAView();

            // toolstrip fetch
            if (force || tsBar.Self == null)
                tsBar.Self = FetchService.FetchTSBar();
            if (force || tsBar.MainToolstrip == null)
                tsBar.MainToolstrip = FetchService.FetchTSBarMainToolStrip();
            if (force || tsBar.MainToolstripItems == null)
                tsBar.MainToolstripItems = FetchService.FetchTSBarMainStoolStripItemsAll(tsBar.MainToolstrip);
            if (force || tsBar.MainToolstripItemsSelectable == null)
                tsBar.MainToolstripItemsSelectable = FetchService.FetchTSBarMainToolStripItemsSelectable(tsBar.MainToolstripItems);

            // toolstrip tabs fetch
            if (force || tsContainer.Self == null)
                tsContainer.Self = FetchService.FetchTSTabContainer();
            if (force || tsContainer.GTSContainer == null)
                tsContainer.GTSContainer = FetchService.FetchGTSContainer(tsContainer.Self);
            if (force || tsContainer.GTSContainerScrollablePanelItems == null)
                tsContainer.GTSContainerScrollablePanelItems = FetchService.FetchTSPanelScrollablePanelItems(tsContainer.GTSContainer);
            if (force || tsContainer.GeneratedScrollPanels == null)
                tsContainer.GeneratedScrollPanels = FetchService.FetchGeneratedScrollPanels();

            // toolstrip tab groups fetch
            if (force || tsTabGroup.GroupToolstrip == null && tsContainer.Self.selectedIndex != -1)
                tsTabGroup.GroupToolstrip = FetchService.FetchGroupToolstrip(tsContainer.Self);

            // special buttons fetch
            if (force || specialUIButtons.Esc == null)
                specialUIButtons.Esc = FetchService.FetchEscButton();
            if (force || specialUIButtons.TSClose == null)
                specialUIButtons.TSClose = FetchService.FetchTSCloseButton();
            if (force || specialUIButtons.Freecamera == null)
                specialUIButtons.Freecamera = FetchService.FetchFreecameraButton();
            if (force || specialUIButtons.Bulldozer == null)
                specialUIButtons.Bulldozer = FetchService.FetchBulldozerButton();
            if (force || specialUIButtons.Speed == null)
                specialUIButtons.Speed = FetchService.FetchSpeedButton();
            if (force || specialUIButtons.Play == null)
                specialUIButtons.Play = FetchService.FetchPlayButton();

            // fetch cursor tools
            if (cursorTools.NetTools == null)
                cursorTools.NetTools = FetchService.FetchNetTools();

            LoadCustomUIComponents();
        }

        private void LoadCustomUIComponents()
        {
            try
            {
                tsBar.ToolstripSelectionSprite = AView.FindUIComponent("ToolstripSelectionSprite");
                tsContainer.ToolbarTabItemSelectionSprite = AView.FindUIComponent("ToolbarTabItemSelectionSprite");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (tsBar.ToolstripSelectionSprite == null)
            {
                AView.AddUIComponent(typeof(ToolstripSelectionSprite));
                tsBar.ToolstripSelectionSprite = AView.FindUIComponent("ToolstripSelectionSprite");
            }

            if (tsContainer.ToolbarTabItemSelectionSprite == null)
            {
                AView.AddUIComponent(typeof(ToolbarTabItemSelectionSprite));
                tsContainer.ToolbarTabItemSelectionSprite = AView.FindUIComponent("ToolbarTabItemSelectionSprite");
            }
        }

        private void HideCustomUIComponentOnMouseHover(UIComponent component)
        {
            var cursorPos = MouseOperations.GetCursorPosition();
            if (component.containsMouse)
                component.zOrder = 0;
            else
            {
                if (cursorPos.X < component.absolutePosition.x
                    || cursorPos.X > component.absolutePosition.x + component.size.x)
                {
                    component.zOrder = 25;
                }
            }
        }

        public static void HideCustomUIComponents()
        {
            UIView aView = UIView.GetAView();
            string[] componentNames = new string[] { "ToolstripSelectionSprite", "ToolbarTabItemSelectionSprite" };
            foreach (var name in componentNames)
            {
                try
                {
                    aView.FindUIComponent(name).Hide();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        public static void OnReleased()
        {
            UINavigationService.HideCustomUIComponents();
            UINavigationService.ResetToolstripBarIndexes();
            LoadingActions.UIReloadNeeded = true;
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
