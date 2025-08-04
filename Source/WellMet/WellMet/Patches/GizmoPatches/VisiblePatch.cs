using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.GizmoPatches {
	[HarmonyPatch(typeof(Gizmo), nameof(Gizmo.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		private static readonly FieldInfo ResourceGeneField = AccessTools.Field(typeof(GeneGizmo_Resource), "gene");

		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(PsychicEntropyGizmo), "tracker");

		private static readonly MethodInfo PawnOwnerMethod = AccessTools.PropertyGetter(typeof(CompShield), "PawnOwner");

		private static readonly FieldInfo DeathrestGeneField = AccessTools.Field(typeof(GeneGizmo_DeathrestCapacity), "gene");

		[HarmonyPostfix]
		private static void Postfix(Gizmo __instance, ref bool __result) {
			if (__instance is Command_VerbTarget commandVerbTarget) {
				__result = __result
					&& (!commandVerbTarget.verb.CasterIsPawn
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, commandVerbTarget.verb.CasterPawn, true));
				return;
			}

			if (__instance is Command_Psycast commandPsycast) {
				__result = __result
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
					&& (commandPsycast.Ability.pawn == null
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, commandPsycast.Ability.pawn, true));
#else
					&& (commandPsycast.Pawn == null
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, commandPsycast.Pawn, true));
#endif
				return;
			}

			if (__instance is GeneGizmo_Resource geneGizmoResource) {
				__result = __result
					&& (!(ResourceGeneField.GetValue(geneGizmoResource) is Gene_Resource geneResource)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, geneResource.pawn, true));
				return;
			}

			if (__instance is PsychicEntropyGizmo psychicEntropyGizmo) {
				__result = __result
					&& (!(TrackerField.GetValue(psychicEntropyGizmo) is Pawn_PsychicEntropyTracker pawnPsychicEntropyTracker)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnPsychicEntropyTracker.Pawn, true));
				return;
			}

			if (__instance is Gizmo_EnergyShieldStatus gizmoEnergyShieldStatus) {
				__result = __result
					&& (!(PawnOwnerMethod.Invoke(gizmoEnergyShieldStatus.shield, Array.Empty<object>()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, true));
				return;
			}

			if (__instance is GeneGizmo_DeathrestCapacity geneGizmoDeathrestCapacity) {
				__result = __result
					&& (!(DeathrestGeneField.GetValue(geneGizmoDeathrestCapacity) is Gene_Deathrest geneDeathrest)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, geneDeathrest.pawn, true));
			}
		}
	}
}
