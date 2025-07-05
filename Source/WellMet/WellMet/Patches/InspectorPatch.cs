using HarmonyLib;
using RimWorld;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(MainTabWindow_Inspect), nameof(MainTabWindow_Inspect.DoWindowContents))]
	internal static class InspectorPatch {
		[HarmonyPrefix]
		private static bool Prefix() => false;
	}
}
