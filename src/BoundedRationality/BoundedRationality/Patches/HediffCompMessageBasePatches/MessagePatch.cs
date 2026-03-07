#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompMessageBasePatches {
	[HarmonyPatch(typeof(HediffComp_MessageBase), "Message")]
	internal static class MessagePatch {
		[HarmonyPrefix]
		private static bool Prefix(HediffComp_MessageBase __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.Pawn, ControlCategory.Message);
	}
}
#endif
