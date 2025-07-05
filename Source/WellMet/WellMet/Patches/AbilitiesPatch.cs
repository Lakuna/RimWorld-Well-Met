using HarmonyLib;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(Gizmo), nameof(Gizmo.Visible), MethodType.Getter)]
	internal static class AbilitiesPatch {
		[HarmonyPrefix]
		private static bool Prefix(ref bool __result) {
			__result = false;
			return false;
		}
	}
}
