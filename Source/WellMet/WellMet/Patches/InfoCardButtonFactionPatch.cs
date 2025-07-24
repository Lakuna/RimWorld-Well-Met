using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Faction) })]
	internal static class InfoCardButtonFactionPatch {
		[HarmonyPrefix]
		public static bool Prefix(Faction faction) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, faction);
	}
}
