#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnLogPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Log), "FillTab")]
	internal static class FillTabPatch {
		private static readonly MethodInfo SelPawnForCombatInfoMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");

		[HarmonyPrefix]
		private static void Prefix(ITab_Pawn_Log __instance, ref bool ___showCombat, ref bool ___showSocial) {
			if (!(SelPawnForCombatInfoMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)) {
				return;
			}

			___showCombat = ___showCombat && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
			___showSocial = ___showSocial && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
		}
	}
}
