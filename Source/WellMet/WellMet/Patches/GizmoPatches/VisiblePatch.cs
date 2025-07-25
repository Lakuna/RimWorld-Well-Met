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
			// Don't show the gizmo if it was already hidden.
			if (!__result) {
				return;
			}

			// Weapon gizmo.
			if (__instance is Command_VerbTarget weaponGizmo) {
				// If there is no selected pawn, do nothing.
				if (!weaponGizmo.verb.CasterIsPawn) {
					return;
				}

				// Show the weapon gizmo only if any of the information on the gizmo is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, weaponGizmo.verb.CasterPawn);
				return;
			}

			// Gene resource gizmo.
			if (__instance is GeneGizmo_Resource geneResourceGizmo) {
				// If there is no selected pawn, do nothing.
				if (!(ResourceGeneField.GetValue(geneResourceGizmo) is Gene_Resource geneResource)) {
					return;
				}

				// Show the gene resource gizmo only if any of the information on the gizmo is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, geneResource.pawn);
				return;
			}

			// Psychic entropy gizmo.
			if (__instance is PsychicEntropyGizmo psychicEntropyGizmo) {
				// If there is no selected pawn, do nothing.
				if (!(TrackerField.GetValue(psychicEntropyGizmo) is Pawn_PsychicEntropyTracker psychicEntropyTracker)) {
					return;
				}

				// Show the psychic entropy gizmo only if any of the information on the gizmo is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, psychicEntropyTracker.Pawn);
				return;
			}

			// Shield energy status gizmo.
			if (__instance is Gizmo_EnergyShieldStatus energyShieldStatusGizmo) {
				// If there is no selected pawn, do nothing.
				if (!(PawnOwnerMethod.Invoke(energyShieldStatusGizmo.shield, Array.Empty<object>()) is Pawn pawn)) {
					return;
				}

				// Show the energy shield status gizmo only if any of the information on the gizmo is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn);
				return;
			}

			// Deathrest capacity gizmo.
			if (__instance is GeneGizmo_DeathrestCapacity deathrestCapacityGizmo) {
				// If there is no selected pawn, do nothing.
				if (!(DeathrestGeneField.GetValue(deathrestCapacityGizmo) is Gene_Deathrest geneDeathrest)) {
					return;
				}

				// Show the deathrest capacity gizmo only if any of the information on the gizmo is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, geneDeathrest.pawn);
			}
		}
	}
}
