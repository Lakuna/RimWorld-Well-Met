using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), nameof(Thought_SituationalSocial.VisibleInNeedsTab), MethodType.Getter)]
	internal static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Thought_SituationalSocial __instance, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance?.pawn ?? throw new ArgumentNullException(nameof(__instance)));
	}
}
