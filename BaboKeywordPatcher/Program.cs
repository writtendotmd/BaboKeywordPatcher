using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;

namespace BaboKeywordPatcher
{
    public class BaboSettings
    {
        public bool ArmorPrettyDefault;
        public bool ArmorEroticDefault;
        public bool EroticDresses;
    }
    
    public class Program
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        static Lazy<BaboSettings> Settings = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(
                    nickname: "Settings",
                    path: "settings.json",
                    out Settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "BaboKeywords.esp")
                .Run(args);
        }

        public static IKeywordGetter LoadKeyword(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, String kwd)
        {
            state.LinkCache.TryResolve<IKeywordGetter>(kwd, out var ReturnKwd);
            if (ReturnKwd == null)
            {
                throw new Exception("Failed to load keyword " + kwd);
            }
            return ReturnKwd;
        }

        public static bool StrMatch(String name, String comparator)
        {
            return (name.IndexOf(comparator, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public static bool StrMatchCS(String name, String comparator)
        {
            return (name.IndexOf(comparator) >= 0);
        }

        public static bool IsDeviousRenderedItem(String name)
        {
            return (StrMatch(name, "scriptinstance") || StrMatch(name, "rendered"));
        }

        public static IKeywordGetter? EroticArmor;
        public static IKeywordGetter? SLA_ArmorHarness;
        public static IKeywordGetter? SLA_ArmorSpendex;
        public static IKeywordGetter? SLA_ArmorTransparent;
        public static IKeywordGetter? SLA_BootsHeels;
        public static IKeywordGetter? SLA_VaginalDildo;
        public static IKeywordGetter? SLA_AnalPlug;
        public static IKeywordGetter? SLA_PiercingClit;
        public static IKeywordGetter? SLA_PiercingNipple;
        public static IKeywordGetter? SLA_ArmorPretty;
        public static IKeywordGetter? SLA_ArmorBondage;
        public static IKeywordGetter? SLA_AnalPlugTail;
        public static IKeywordGetter? SLA_AnalBeads;
        public static IKeywordGetter? SLA_VaginalBeads;
        public static IKeywordGetter? SLA_ArmorRubber;
        public static IKeywordGetter? SLA_BraArmor;
        public static IKeywordGetter? SLA_ThongT;
        public static IKeywordGetter? SLA_PantiesNormal;
        public static IKeywordGetter? SLA_HasLeggings;
        public static IKeywordGetter? SLA_HasStockings;
        public static IKeywordGetter? SLA_MiniSkirt;
        public static IKeywordGetter? SLA_ArmorHalfNakedBikini;

        public static void LoadKeywords(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            SLA_ArmorHarness = LoadKeyword(state, "SLA_ArmorHarness");
            try // SLAX vs SLA Babo spell this keyword differently. Check for both.
            {
                SLA_ArmorSpendex = LoadKeyword(state, "SLA_ArmorSpendex");
            }
            catch
            {
                SLA_ArmorSpendex = LoadKeyword(state, "SLA_ArmorSpandex");
            }
            SLA_ArmorTransparent = LoadKeyword(state, "SLA_ArmorTransparent");
            SLA_BootsHeels = LoadKeyword(state, "SLA_BootsHeels");
            SLA_VaginalDildo = LoadKeyword(state, "SLA_VaginalDildo");
            SLA_AnalPlug = LoadKeyword(state, "SLA_AnalPlug");
            SLA_PiercingClit = LoadKeyword(state, "SLA_PiercingClit");
            SLA_PiercingNipple = LoadKeyword(state, "SLA_PiercingNipple");
            SLA_ArmorPretty = LoadKeyword(state, "SLA_ArmorPretty");
            EroticArmor = LoadKeyword(state, "EroticArmor");
            SLA_ArmorBondage = LoadKeyword(state, "SLA_ArmorBondage");
            SLA_AnalPlugTail = LoadKeyword(state, "SLA_AnalPlugTail");
            SLA_AnalBeads = LoadKeyword(state, "SLA_AnalPlugBeads");
            SLA_VaginalBeads = LoadKeyword(state, "SLA_VaginalBeads");
            SLA_ArmorRubber = LoadKeyword(state, "SLA_ArmorRubber");
            try
            {
                SLA_BraArmor = LoadKeyword(state, "SLA_BraArmor");
            }
            catch
            {
                SLA_BraArmor = LoadKeyword(state, "SLA_Brabikini");
            }
            SLA_ThongT = LoadKeyword(state, "SLA_ThongT");
            SLA_PantiesNormal = LoadKeyword(state, "SLA_PantiesNormal");
            SLA_HasLeggings = LoadKeyword(state, "SLA_HasLeggings");
            SLA_HasStockings = LoadKeyword(state, "SLA_HasStockings");
            SLA_MiniSkirt = LoadKeyword(state, "SLA_MiniSkirt");
            SLA_ArmorHalfNakedBikini = LoadKeyword(state, "SLA_ArmorHalfNakedBikini");
        }

        private static void AddTag(Armor AEO, IKeywordGetter tag)
        {
            System.Console.WriteLine("Added keyword " + tag.ToString() + " to armor " + AEO.Name);
            if (AEO.Keywords == null)
            {
                System.Console.WriteLine("AOE.Keywords == null: " + AEO);
                // AEO.Keywords!.Add(tag);
            }
            else
            {
                if (!AEO.Keywords.Contains(tag))
                {
                    AEO.Keywords!.Add(tag);
                }
            }
        }

        // Keywords are static / nullabe, but are initialized on runtime. Ignore warning.
#pragma warning disable CS8604 // Possible null reference argument.
        public static void ParseName(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IArmorGetter armor, String name)
        {
            bool matched = false;
            //var armorEditObj = state.PatchMod.Armors.GetOrAddAsOverride(armor);
            var armorEditObj = armor.DeepCopy();

            if (armorEditObj == null)
            {
                System.Console.WriteLine("Armor is null for " + name);
                return;
            }
            // EroticArmor
            if (StrMatch(name, "harness") || StrMatch(name, "corset") || StrMatch(name, "StraitJacket") || 
                StrMatch(name, "hobble") || StrMatch(name, "tentacles") || 
                StrMatch(name, "slave") || StrMatch(name, "chastity") || StrMatch(name, "cuff") || StrMatch(name, "binder") ||
                StrMatch(name, "yoke") || StrMatch(name, "mitten")
                )
            {
                matched = true;
                AddTag(armorEditObj, SLA_ArmorBondage);
            }

            // EroticArmor
            if (StrMatch(name, "suit") ||  StrMatch(name, "latex") || StrMatch(name, "rubber") ||
                StrMatch(name, "ebonite") || StrMatch(name, "slut") || StrMatch(name, "lingerie") ||
                (StrMatch(name, "dress") && Settings.Value.EroticDresses)
                )
            {
                matched = true;
                AddTag(armorEditObj, EroticArmor);
            }
            // SLA_ArmorRubber
            if (StrMatch(name, "rubber"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_ArmorRubber);
            }

            //SLA_ArmorHarness
            if (StrMatch(name, "harness"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_ArmorHarness);
            }
            // SLA_ArmorSpendex
            if (StrMatch(name, "suit") || StrMatch(name, "spandex") || StrMatch(name, "spendex") || StrMatch(name, "ebonite"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_ArmorSpendex);
            }
            // SLA_ArmorTransparent
            if (StrMatch(name, "transparent") || StrMatchCS(name, "TR"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_ArmorTransparent);
            }
            // SLA_BootsHeels
            IBodyTemplateGetter? bodyTemplate = armor.BodyTemplate;
            if ((IsDeviousRenderedItem(name) && StrMatch(name, "boots")) || 
                (StrMatch(name, "heels") && !StrMatch(name, "wheel") && 
                bodyTemplate != null && bodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)))
            {
                matched = true;
                AddTag(armorEditObj, SLA_BootsHeels);
            }
            //SLA_VaginalDildo
            if ((StrMatch(name, "plug") && StrMatch(name, "vag")) || StrMatch(name, "vaginal") || StrMatch(name, "vibrator"))
            {
                matched = true;
                if (StrMatch(name, "beads"))
                {
                    AddTag(armorEditObj, SLA_VaginalBeads);
                }
                else
                {
                    AddTag(armorEditObj, SLA_VaginalDildo);
                }
            }
            // SLA_AnalPlug
            if (StrMatch(name, "anal") || StrMatch(name, "buttplug") || StrMatch(name, "vibrator"))
            {
                matched = true;
                if (StrMatch(name, "tail"))
                {
                    AddTag(armorEditObj, SLA_AnalPlugTail);
                }
                else if (StrMatch(name, "beads")) 
                {
                    AddTag(armorEditObj, SLA_AnalBeads);
                }
                else
                {
                    AddTag(armorEditObj, SLA_AnalPlug);
                }
                
            }
            // SLA_PiercingClit
            if (StrMatch(name, "piercingv") || StrMatch(name, "vpiercing"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_PiercingClit);
            }
            // SLA_PiercingNipple
            if (StrMatch(name, "piercingn") || StrMatch(name, "npiercing"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_PiercingNipple);
            }
            // SLA_BraArmor
            if (!StrMatch(name, "bracer") && !StrMatch(name, "brawn") && (StrMatch(name, " bra") || StrMatch(name, "bikini top") || 
                (StrMatch(name, "undergarment") && StrMatch(name, "upper"))))
            {
                matched = true;
                AddTag(armorEditObj, SLA_BraArmor);
            }
            if (StrMatch(name, "bikini"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_ArmorHalfNakedBikini);
            }
            // SLA_ThongT
            if (StrMatch(name, "thong") || StrMatch(name, "bottom"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_ThongT);
            }
            //SLA_PantiesNormal
            if (StrMatch(name, "panties") || StrMatch(name, "panty") || StrMatch(name, "underwear") || StrMatch(name, "binkini bot") || 
                StrMatch(name, "pants") || (StrMatch(name, "undergarment")  && StrMatch(name, "lower")))
            {
                matched = true;
                AddTag(armorEditObj, SLA_PantiesNormal);
            }
            //SLA_HasStockings
            if (StrMatch(name, "stockings"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_HasStockings);
            }
            //SLA_HasLeggings
            if (StrMatch(name, "leggings"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_HasLeggings);
            }
            //SLA_HasLeggings
            if (StrMatch(name, "skirt"))
            {
                matched = true;
                AddTag(armorEditObj, SLA_MiniSkirt);
            }
            // All vanilla armors
            if (Settings.Value.ArmorPrettyDefault && !matched && (StrMatch(name, "armor") || StrMatch(name, "cuiras") || StrMatch(name, "robes")))
            { // I use a skimpy armor replacer (But not to the level of bikini). Having ArmorPretty on all armors is appropriate.
                matched = true;
                AddTag(armorEditObj, SLA_ArmorPretty);
            }
            else if (Settings.Value.ArmorEroticDefault && !matched && (StrMatch(name, "armor") || StrMatch(name, "cuiras") || StrMatch(name, "robes")))
            { 
                matched = true;
                AddTag(armorEditObj, EroticArmor);
            }
            if (matched)
            {
                // System.Console.WriteLine("Matched: " + name);
                state.PatchMod.Armors.Set(armorEditObj);
            }
        }
#pragma warning restore CS8604 // Possible null reference argument.

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            //Your code here!
            LoadKeywords(state);
            // state.ExtraSettingsDataPath.
            foreach (var armorGetter in state.LoadOrder.PriorityOrder.WinningOverrides<IArmorGetter>())
            {
                try
                {
                    // skip armor with non-default race
                    if (armorGetter.Race != null)
                    {
                        armorGetter.Race.TryResolve<IRaceGetter>(state.LinkCache, out var race);
                        if (race != null && race.EditorID != "DefaultRace") continue;
                    }
                    // skip armor that is non-playable or a shield
                    if (armorGetter.MajorFlags.HasFlag(Armor.MajorFlag.NonPlayable)) continue;
                    if (armorGetter.MajorFlags.HasFlag(Armor.MajorFlag.Shield)) continue;
                    // skip armor that is head, hair, circlet, hands, feet, rings, or amulets
                    if (armorGetter.BodyTemplate != null)
                    {
                        if (armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Head)) continue;
                        if (armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Hair)) continue;
                        if (armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Circlet)) continue;
                        //if (armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Hands)) continue; // - Mittens
                        if (armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Ring)) continue;
                        if (armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Amulet)) continue;
                    }
                    if (armorGetter.Keywords == null) continue;

                    if (armorGetter.Name != null)
                    {
                        string? v = armorGetter.Name.ToString();
                        if (v != null)
                        {
                            ParseName(state, armorGetter, v);
                        }
                    }
                    else
                    {
                        if (armorGetter.EditorID != null)
                        {
                            ParseName(state, armorGetter, armorGetter.EditorID);
                        }
                    }
                }
                // MoreNastyCritters breaks the patching process. Ignore it.
                catch (Exception e)
                {
                    System.Console.WriteLine("Caught exception: " + e);
                }
            }
            System.Console.WriteLine("Done.");
        }
    }
}
