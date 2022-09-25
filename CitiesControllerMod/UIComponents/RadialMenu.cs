using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesControllerMod.UIComponents
{
    public class RadialMenuItem
    {
        private string displayText;
        private string iconSprite;
        private string action;

        public RadialMenuItem(string displayText, string iconSprite, string action)
        {
            this.displayText = displayText;
            this.iconSprite = iconSprite;
            this.action = action;
        }
        public string DisplayText { get => displayText; set => displayText = value; }
        public string IconSprite { get => iconSprite; set => iconSprite = value; }
        public string Action { get => action; set => this.action = value; }
    }
    public enum RadialMenuStatus { Setup, Created }
    public class RadialMenu
    {
        // private properties

        List<RadialMenuItem> items = new List<RadialMenuItem>();
        UIComponent radialMenuUIComponent;
        private RadialMenuStatus status = RadialMenuStatus.Setup;
        int selectedIndex = -1;
        int radius = 300;
        private bool isOpen = false;
        private bool firstUpdateCycleDone = false; // first ui update cycle might generate components we cannot refer to. better skip that

        // public properties

        public static string NoActionName = "NoAction";
        public string GetSelectedAction() { return selectedIndex > -1 ? items[selectedIndex].Action : NoActionName; }
        public int OptionsCount { get { return items.Count(); } }
        public bool IsOpen { get => isOpen; }
        public void SetIsOpen(bool isOpen)
        {
            this.isOpen = isOpen;
            UpdateVisibility();
        }
        public List<RadialMenuItem> Items { get => items; set => SetItems(value); }
        public void SetItems(List<RadialMenuItem> items)
        {
            this.items = items;
            status = RadialMenuStatus.Setup;
        }

        // functions

        public void UpdateUIComponent(UIView aView) // keep running if open
        {
            // ensuring that there's always a RadialMenuMainComponent
            radialMenuUIComponent = UIView.Find("RadialMenuMainComponent");
            if (radialMenuUIComponent == null)
            {
                aView.AddUIComponent(typeof(RadialMenuMainComponent));
                radialMenuUIComponent = aView.FindUIComponent("RadialMenuMainComponent");
            }
            else
            {
                // once we got a hold of a RadialMenuMainComponent, we fill it with RadialMenuSelectionComponents and assign their values
                while (radialMenuUIComponent.components.Count() < OptionsCount)
                {
                    radialMenuUIComponent.AddUIComponent(typeof(RadialMenuSelectionComponent));
                }
                if (firstUpdateCycleDone)
                {
                    switch (status) // assigning selectioncomponent properties and making the correct ones visible
                    {
                        case RadialMenuStatus.Setup:
                            SetupRadialMenuOptions();
                            break;
                        case RadialMenuStatus.Created:
                            UpdateVisibility();
                            break;
                    }
                }
                firstUpdateCycleDone = true;
            }
        }

        private void SetupRadialMenuOptions()
        {
            if (radialMenuUIComponent != null && OptionsCount > 0)
            {
                for (int i = 0; i < radialMenuUIComponent.components.Count(); ++i)
                {
                    UIComponent currentComponent = radialMenuUIComponent.components[i];

                    if (i < OptionsCount) // calculating only the selectioncomponents with an equivalent in items[]
                    {
                        RadialMenuItem currentItem = items[i];
                        UILabel currentLabel = currentComponent.components[0] as UILabel;
                        UIPanel currentIcon = currentComponent.components[1] as UIPanel;

                        // setting properties
                        currentLabel.text = currentItem.DisplayText;
                        currentIcon.backgroundSprite = currentItem.IconSprite;
                        currentComponent.enabled = true;

                        // calculating position
                        float circleposition = (float)i / (float)OptionsCount;
                        float x, y;
                        if (OptionsCount == 6)
                        {
                            x = Mathf.Sin(circleposition * Mathf.PI * 2.0f) * radius;
                            y = Mathf.Cos(circleposition * Mathf.PI * 2.0f) * radius;
                            currentComponent.position = new Vector2(y - currentComponent.size.x / 2, x + currentComponent.size.y / 2);
                        }
                        else
                        {
                            x = Mathf.Cos(circleposition * Mathf.PI * 2.0f) * radius;
                            y = Mathf.Sin(circleposition * Mathf.PI * 2.0f) * radius;
                            currentComponent.position = new Vector2(y - currentComponent.size.x / 2, x + currentComponent.size.y / 2);
                        }
                    }
                    else // hiding selectioncomponents generated by previously opened radial menus that had more options (ui component removal is unsafe)
                    {
                        currentComponent.enabled = false;
                    }
                }
                status = RadialMenuStatus.Created;
            }
        }

        public void UpdateRadialMenuSelection(Vector2 analogStickPos) // keep running if open
        {
            if (status == RadialMenuStatus.Created)
            {
                // processing analog stick input
                double deadzone = 0.5;
                bool selecting = analogStickPos.x > deadzone || analogStickPos.x < -deadzone || analogStickPos.y > deadzone || analogStickPos.y < -deadzone;

                // calculating the selected item index
                if (!selecting)
                {
                    selectedIndex = -1;
                }
                else
                {
                    float result = 0;
                    if (OptionsCount == 6)
                    {
                        result = (Mathf.Atan2(-analogStickPos.y, -analogStickPos.x) * Mathf.Rad2Deg) + 180;
                    }
                    else
                    {
                        result = (Mathf.Atan2(-analogStickPos.x, -analogStickPos.y) * Mathf.Rad2Deg) + 180;
                    }
                    selectedIndex = (int)Math.Round(result / (360 / OptionsCount));
                    if (selectedIndex == OptionsCount)
                    {
                        selectedIndex = 0;
                    }
                }

                // updating selectioncomponent display colours to reflect which one's selected
                for (int i = 0; i < OptionsCount; i++)
                {
                    if (selecting)
                    {
                        if (selecting && i == selectedIndex)
                        {
                            radialMenuUIComponent.components[selectedIndex].color = RadialMenuSelectionComponent.SelectedColor;
                        }
                        else if (selecting && selectedIndex == OptionsCount)
                        {
                            radialMenuUIComponent.components[0].color = RadialMenuSelectionComponent.SelectedColor;
                            radialMenuUIComponent.components[selectedIndex - 1].color = RadialMenuSelectionComponent.DefaultColor;
                        }
                        else
                        {
                            radialMenuUIComponent.components[i].color = RadialMenuSelectionComponent.DefaultColor;
                        }
                    }
                    else
                    {
                        radialMenuUIComponent.components[i].color = RadialMenuSelectionComponent.DefaultColor;
                    }
                }
            }
        }

        private void UpdateVisibility()
        {
            if (radialMenuUIComponent != null)
            {
                radialMenuUIComponent.enabled = IsOpen;
                for (int i = 0; i < OptionsCount; i++)
                {
                    radialMenuUIComponent.components[i].enabled = IsOpen;
                }
            }
        }
    }
}
