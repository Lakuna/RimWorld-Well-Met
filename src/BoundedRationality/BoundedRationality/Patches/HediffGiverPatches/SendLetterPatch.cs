#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.HediffGiverPatches {
	[HarmonyPatch(typeof(HediffGiver), "SendLetter")]
	internal static class SendLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Letter);
	}
}
