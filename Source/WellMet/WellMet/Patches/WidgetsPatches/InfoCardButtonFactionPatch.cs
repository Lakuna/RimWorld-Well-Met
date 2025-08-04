#if !V1_0
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.WidgetsPatches {
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Faction) })]
	internal static class InfoCardButtonFactionPatch {
		[HarmonyPrefix]
		private static bool Prefix(Faction faction) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, faction, true);
	}
}
#endif
