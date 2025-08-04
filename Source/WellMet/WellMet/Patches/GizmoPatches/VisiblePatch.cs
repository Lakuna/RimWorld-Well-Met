#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !V1_0
using System;
#endif
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.GizmoPatches {
	[HarmonyPatch(typeof(Gizmo), nameof(Gizmo.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo ResourceGeneField = AccessTools.Field(typeof(GeneGizmo_Resource), "gene");
#endif

#if !V1_0
		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(PsychicEntropyGizmo), "tracker");
#endif

#if V1_0
		private static readonly MethodInfo WearerMethod = AccessTools.Method(typeof(Apparel), "get_" + nameof(Apparel.Wearer));
#elif V1_1 || V1_2 || V1_3
		private static readonly MethodInfo WearerMethod = AccessTools.PropertyGetter(typeof(Apparel), nameof(Apparel.Wearer));
#else
		private static readonly MethodInfo PawnOwnerMethod = AccessTools.PropertyGetter(typeof(CompShield), "PawnOwner");
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo DeathrestGeneField = AccessTools.Field(typeof(GeneGizmo_DeathrestCapacity), "gene");
#endif

#if V1_1
		private static readonly FieldInfo AbilityField = AccessTools.Field(typeof(Command_Ability), "ability");
#endif

#if V1_0
		internal static readonly object[] Parameters = new object[] { };
#endif

		[HarmonyPostfix]
		private static void Postfix(Gizmo __instance, ref bool __result) {
			if (__instance is Command_VerbTarget commandVerbTarget) {
				__result = __result
					&& (!commandVerbTarget.verb.CasterIsPawn
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, commandVerbTarget.verb.CasterPawn, true));
				return;
			}

#if !V1_0
			if (__instance is Command_Psycast commandPsycast) {
				__result = __result
#if V1_1
					&& (!(AbilityField.GetValue(commandPsycast) is Ability ability)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, ability.pawn, true));
#elif V1_2 || V1_3 || V1_4
					&& (commandPsycast.Ability.pawn == null
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, commandPsycast.Ability.pawn, true));
#else
					&& (commandPsycast.Pawn == null
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, commandPsycast.Pawn, true));
#endif
				return;
			}
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			if (__instance is GeneGizmo_Resource geneGizmoResource) {
				__result = __result
					&& (!(ResourceGeneField.GetValue(geneGizmoResource) is Gene_Resource geneResource)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, geneResource.pawn, true));
				return;
			}
#endif

#if !V1_0
			if (__instance is PsychicEntropyGizmo psychicEntropyGizmo) {
				__result = __result
					&& (!(TrackerField.GetValue(psychicEntropyGizmo) is Pawn_PsychicEntropyTracker pawnPsychicEntropyTracker)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnPsychicEntropyTracker.Pawn, true));
				return;
			}
#endif

			if (__instance is Gizmo_EnergyShieldStatus gizmoEnergyShieldStatus) {
				__result = __result
#if V1_0
					&& (!(WearerMethod.Invoke(gizmoEnergyShieldStatus.shield, Parameters) is Pawn pawn)
#elif V1_1 || V1_2 || V1_3
					&& (!(WearerMethod.Invoke(gizmoEnergyShieldStatus.shield, Array.Empty<object>()) is Pawn pawn)
#else
					&& (!(PawnOwnerMethod.Invoke(gizmoEnergyShieldStatus.shield, Array.Empty<object>()) is Pawn pawn)
#endif
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, true));
				return;
			}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			if (__instance is GeneGizmo_DeathrestCapacity geneGizmoDeathrestCapacity) {
				__result = __result
					&& (!(DeathrestGeneField.GetValue(geneGizmoDeathrestCapacity) is Gene_Deathrest geneDeathrest)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, geneDeathrest.pawn, true));
			}
#endif
		}
	}
}
