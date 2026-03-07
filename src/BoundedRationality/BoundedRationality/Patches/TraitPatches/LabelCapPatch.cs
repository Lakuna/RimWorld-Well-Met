#if !(V1_0 || V1_1)
using HarmonyLib;

using Lakuna.WellMet.Utility;

using RimWorld;

using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.LabelCap), MethodType.Getter)]
	internal static class LabelCapPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Trait __instance, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsTraitKnown(__instance)) {
				return;
			}

			__result = "BR.Unknown".Translate().CapitalizeFirst();
		}
	}
}
#endif
