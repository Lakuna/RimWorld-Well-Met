using HarmonyLib;

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.TimeSlowerPatches {
	[HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeedShort))]
	internal static class SignalForceNormalSpeedShortPatch {
		[HarmonyPrefix]
		private static bool Prefix() => !WellMetMod.Settings.PreventForcedSpeed || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, PawnType.Colonist);
	}
}
