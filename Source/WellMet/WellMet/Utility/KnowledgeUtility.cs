using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Utility {
	public static class KnowledgeUtility {
		private const int TicksPerYear = 4 * 15 * 24 * 2500;

		public static PawnType TypeOf(Pawn pawn) =>
			pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.IsSlave
			? PawnType.Slave
			: pawn.IsPrisoner
			? PawnType.Prisoner
			: pawn.IsColonist
			? PawnType.Colonist
			: PawnType.Other;

		public static bool IsInformationKnownFor(InformationCategory informationCategory, Pawn pawn) =>
			IsInformationKnownFor(informationCategory, TypeOf(pawn));

		public static bool IsInformationKnownFor(InformationCategory informationCategory, PawnType pawnType) =>
			WellMetMod.Settings.KnownInformation[(int)pawnType, (int)informationCategory];

		public static bool IsAllInformationKnownFor(Pawn pawn) =>
			IsAllInformationKnownFor(TypeOf(pawn));

		public static bool IsAllInformationKnownFor(PawnType pawnType) =>
			((InformationCategory[])Enum.GetValues(typeof(InformationCategory)))
			.Any((InformationCategory informationCategory) => !IsInformationKnownFor(informationCategory, pawnType));

		public static bool IsTraitKnown(Trait trait) {
			if (trait == null) {
				throw new ArgumentNullException(nameof(trait));
			}

			return IsTraitKnown(trait.pawn, trait.def);
		}

		public static bool IsTraitKnown(Pawn pawn, TraitDef traitDef) {
			if (pawn == null) {
				// In vanilla RimWorld, `pawn == null` only during a growth moment.
				return IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist)
					&& (WellMetMod.Settings.AlwaysKnowGrowthMoments || WellMetMod.Settings.TraitDiscoverSpeedFactor <= 0);
			}

			if (traitDef == null) {
				return true;
			}

			PawnType pawnType = TypeOf(pawn);
			if (!IsInformationKnownFor(InformationCategory.Traits, pawnType)) {
				return false;
			}

			// TODO: Required work types.

			// TODO: Required work tags.

			if (pawnType != PawnType.Colonist) {
				return true;
			}

			return pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal)
				> traitDef.GetGenderSpecificCommonality(pawn.gender) * TicksPerYear / WellMetMod.Settings.TraitDiscoverSpeedFactor;
		}

		public static bool IsThoughtKnown(Thought thought) {
			if (thought == null) {
				throw new ArgumentNullException(nameof(thought));
			}

			return IsThoughtKnown(thought.pawn, thought.def);
		}

		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thoughtDef) {
			if (thoughtDef == null) {
				throw new ArgumentNullException(nameof(thoughtDef));
			}

			if (!IsInformationKnownFor(InformationCategory.Mood, pawn)) {
				return false;
			}

			if (thoughtDef.requiredTraits.Any((TraitDef traitDef) => !IsTraitKnown(pawn, traitDef))) {
				return false;
			}

			// TODO: Required trait degrees.

			// TODO: Required genes.

			// TODO: Required hediffs.

			return true;
		}

		public readonly static FieldInfo PawnRoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		public static Pawn_RoyaltyTracker RoyaltyObfuscator(Pawn_RoyaltyTracker royalty) =>
			royalty != null && IsInformationKnownFor(InformationCategory.Advanced, royalty.pawn) ? royalty : null;

		public readonly static MethodInfo RoyaltyObfuscatorInfo = SymbolExtensions.GetMethodInfo(() => RoyaltyObfuscator(null));

		public readonly static FieldInfo PawnGuiltField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt));

		public static Pawn_GuiltTracker GuiltObfuscator(Pawn_GuiltTracker guilt, Pawn pawn) =>
			IsInformationKnownFor(InformationCategory.Basic, pawn) ? guilt : null;

		public readonly static MethodInfo GuiltObfuscatorInfo = SymbolExtensions.GetMethodInfo(() => GuiltObfuscator(null, null));

		public readonly static MethodInfo PawnFactionGetterInfo = SymbolExtensions.GetMethodInfo((Pawn pawn) => pawn.Faction);

		public static Faction FactionObfuscator(Faction faction, Pawn pawn) =>
			IsInformationKnownFor(InformationCategory.Basic, pawn) ? faction : null;

		public readonly static MethodInfo FactionObfuscatorInfo = SymbolExtensions.GetMethodInfo(() => FactionObfuscator(null, null));

		public readonly static FieldInfo PawnGenesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));

		public static Pawn_GeneTracker GeneObfuscator(Pawn_GeneTracker genes) =>
			genes != null && IsInformationKnownFor(InformationCategory.Basic, genes.pawn) ? genes : null;

		public readonly static MethodInfo GeneObfuscatorInfo = SymbolExtensions.GetMethodInfo(() => GeneObfuscator(null));
	}
}
