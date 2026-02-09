using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Satchel.BetterMenus;
using UnityEngineInternal;

namespace GeoMultiplier
{

    public class GlobalSettings
    {
        public float overallMultiplier;
        public float smallGeoMultiplier;
        public float mediumGeoMultiplier;
        public float largeGeoMultiplier;
        public bool roundingMode;
    }

    public class GeoMultiplier : Mod, ICustomMenuMod, IGlobalSettings<GlobalSettings>
    {
        public GeoMultiplier() : base("GeoMultiplier") { }
        public override string GetVersion() => "0.1";
        public static GlobalSettings GS = new GlobalSettings();
        private Menu MenuRef;
        public override void Initialize()
        {
            On.HeroController.AddGeo += AddGeo;
            //ModHooks.SetPlayerIntHook += PlayerIntSet;
        }

        public void AddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
        {

            Log($"Vanilla added amount: {amount}");
            if (amount == 1)
            {
                Log($"multiplied is {amount * GS.smallGeoMultiplier * GS.overallMultiplier}, small multiplier: {GS.smallGeoMultiplier}, overal multiplier: {GS.overallMultiplier}");
                orig(self, Rounding(amount * GS.smallGeoMultiplier * GS.overallMultiplier));
            }
            else if (amount == 5)
            {
                orig(self, Rounding(amount * GS.mediumGeoMultiplier * GS.overallMultiplier));
            }
            else if (amount == 25)
            {
                orig(self, Rounding(amount * GS.largeGeoMultiplier * GS.overallMultiplier));
            }
            else
            {
                orig(self, amount);
            }
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            MenuRef ??= new Menu(
                        name: "GeoMultiplier",
                        elements: new Element[]
                        {
                        new HorizontalOption(
                            name: "Round up or down",
                            description: "Rounding up means 1.1 gets rounded to 2",
                            values: new [] { "Round up", "Round down" },
                            applySetting: index =>
                            {
                                GS.roundingMode = index == 0; //"yes" is the 0th index in the values array
                            },
                            loadSetting: () => GS.roundingMode ? 0 : 1), //return 0 ("Yes") if active and 1 ("No") if false
                        new CustomSlider(
                            name: "Overall Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.overallMultiplier = (float) val;
                            },
                            loadValue: () => GS.overallMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),

                        new CustomSlider(
                            name: "Small Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.smallGeoMultiplier = (float) val;
                            },
                            loadValue: () => GS.smallGeoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),

                        new CustomSlider(
                            name: "Medium Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.mediumGeoMultiplier = (float) val;
                            },
                            loadValue: () => GS.mediumGeoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),

                        new CustomSlider(
                            name: "Large Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.largeGeoMultiplier = (float) val;
                            },
                            loadValue: () => GS.largeGeoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            )
                        }

            );


            return MenuRef.GetMenuScreen(modListMenu);
        }

        public bool ToggleButtonInsideMenu { get; }

        public GlobalSettings OnSaveGlobal() => GS;

        public void OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? new GlobalSettings();
        }

        private int Rounding(float value)
        {
            Log($"Value is : {value}, rounding mode is : {GS.roundingMode}");
            if (!GS.roundingMode)
            {
                return Mathf.FloorToInt(value);
            }
            else
            {
                return Mathf.CeilToInt(value);
            }
        }
    }
}
