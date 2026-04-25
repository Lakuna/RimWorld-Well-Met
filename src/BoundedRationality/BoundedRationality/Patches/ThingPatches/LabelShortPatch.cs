#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ThingPatches {
	[HarmonyPatch(typeof(Thing), nameof(Thing.LabelShort), MethodType.Getter)]
	internal static class LabelShortPatch {
		[HarmonyPostfix]

#pragma warning disable CA1707
		private static void Postfix(Thing __instance, ref string __result) {
#pragma warning restore CA1707
			if (!(__instance is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate();
		}
	}
}
