#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.InspirationPatches {
	[HarmonyPatch(typeof(Inspiration), "AddEndMessage")]
	internal static class AddEndMessagePatch {
		[HarmonyPrefix]
		private static bool Prefix(Inspiration __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.pawn, ControlCategory.Message);
	}
}
