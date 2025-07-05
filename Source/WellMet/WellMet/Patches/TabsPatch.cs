using HarmonyLib;
using RimWorld;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(MainTabWindow_Inspect), nameof(MainTabWindow_Inspect.CurTabs), MethodType.Getter)]
	internal static class TabsPatch {
		[HarmonyPrefix]
		private static bool Prefix() => false;
	}
}
