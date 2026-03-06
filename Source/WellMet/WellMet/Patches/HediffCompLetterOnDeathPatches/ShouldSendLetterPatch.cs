#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.HediffCompLetterOnDeathPatches {
	[HarmonyPatch(typeof(HediffComp_LetterOnDeath), "ShouldSendLetter", MethodType.Getter)]
	internal static class ShouldSendLetterPatch {
		[HarmonyPostfix]
		private static void Postfix(HediffComp_LetterOnDeath __instance, ref bool __result) =>
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.parent.pawn, ControlCategory.Letter);
	}
}
