using HarmonyLib;

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.TimeSlowerPatches {
	[HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeed))]
	internal static class SignalForceNormalSpeedPatch {
		[HarmonyPrefix]
		private static bool Prefix() => !WellMetMod.Settings.PreventForcedSpeed || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, PawnType.Colonist);
	}
}
