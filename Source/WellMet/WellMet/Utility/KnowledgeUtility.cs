using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Lakuna.WellMet.Utility {
	public static class KnowledgeUtility {
		private const int TicksPerQuadrum = 15 * 24 * 2500;

		private const int HumanMaxHealth = 100;

		private const float HumanDailyNutrition = 1.6f;

		public static PawnType TypeOf(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.IsFreeNonSlaveColonist ? PawnType.Colonist
			: pawn.IsPlayerControlled ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: pawn.HostileTo(Faction.OfPlayer) ? PawnType.Hostile
			: PawnType.Neutral;

		public static bool IsPlayerControlled(Pawn pawn) => IsPlayerControlled(TypeOf(pawn));

		public static bool IsPlayerControlled(PawnType type) => type == PawnType.Colonist || type == PawnType.Controlled;

		public static bool IsInformationKnownFor(InformationCategory informationCategory, Pawn pawn) => IsInformationKnownFor(informationCategory, TypeOf(pawn));

		public static bool IsInformationKnownFor(InformationCategory informationCategory, PawnType pawnType) => WellMetMod.Settings.KnownInformation[(int)pawnType, (int)informationCategory];

		public static bool IsAllInformationKnownFor(Pawn pawn) => IsAllInformationKnownFor(TypeOf(pawn));

		public static bool IsAllInformationKnownFor(PawnType pawnType) => ((InformationCategory[])Enum.GetValues(typeof(InformationCategory))).Any((informationCategory) => !IsInformationKnownFor(informationCategory, pawnType));

		public static bool IsTraitKnown(Trait trait) => trait == null
			? throw new ArgumentNullException(nameof(trait))
			: IsTraitKnown(trait.pawn, trait.def);

		public static bool IsTraitKnown(Pawn pawn, TraitDef traitDef) {
			// In vanilla RimWorld, `pawn == null` only during a growth moment.
			if (pawn == null) {
				return IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist) && (WellMetMod.Settings.AlwaysKnowGrowthMoments || WellMetMod.Settings.ColonistTraitDiscoveryDifficulty <= 0);
			}

			// Prevents some issues where some pawns cannot gain traits.
			if (traitDef == null) {
				return true;
			}

			PawnType pawnType = TypeOf(pawn);
			if (!IsInformationKnownFor(InformationCategory.Traits, pawnType)) {
				return false;
			}

			// TODO: Required work types.

			// TODO: Required work tags.

			// Only colonists' traits might need to be learned over time.
			if (pawnType != PawnType.Colonist || WellMetMod.Settings.ColonistTraitDiscoveryDifficulty <= 0) {
				return true;
			}

			// Respect user setting for being able to see traits of starting colonists.
			float timeAsColonist = pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal);
			if (WellMetMod.Settings.AlwaysKnowStartingColonists && timeAsColonist <= 0) {
				return true;
			}

			// Special discovery conditions.

			if (traitDef == TraitDefOf.Bloodlust) {
				return pawn.records.GetValue(RecordDefOf.Kills) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty;
			}

			if (traitDef == TraitDefOf.Pyromaniac) {
				return pawn.records.GetValue(RecordDefOf.TimesInMentalState) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty;
			}

			if (traitDef == TraitDefOf.Brawler || traitDef.defName == "ShootingAccuracy") {
				return pawn.records.GetValue(RecordDefOf.ShotsFired) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty * 10;
			}

			if (traitDef == TraitDefOf.Wimp || traitDef.defName == "Tough" || traitDef.defName == "Masochist") {
				return pawn.records.GetValue(RecordDefOf.DamageTaken) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty * (HumanMaxHealth / 10);
			}

			if (traitDef == TraitDefOf.BodyPurist || traitDef == TraitDefOf.Transhumanist) {
				return pawn.records.GetValue(RecordDefOf.OperationsReceived) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty;
			}

			if (traitDef.defName == "Gourmand") {
				return pawn.records.GetValue(RecordDefOf.NutritionEaten) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty * HumanDailyNutrition * 10;
			}

			// For traits without special discovery conditions, discover based on user settings and time as a colonist.
			return timeAsColonist > 1 / traitDef.GetGenderSpecificCommonality(pawn.gender) * TicksPerQuadrum * WellMetMod.Settings.ColonistTraitDiscoveryDifficulty;
		}

		public static bool IsThoughtKnown(Thought thought) => thought == null
			? throw new ArgumentNullException(nameof(thought))
			: IsThoughtKnown(thought.pawn, thought.def);

		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thoughtDef) => thoughtDef == null
			? throw new ArgumentNullException(nameof(thoughtDef))
			: IsInformationKnownFor(InformationCategory.Needs, pawn) && !thoughtDef.requiredTraits.Any((traitDef) => !IsTraitKnown(pawn, traitDef));
	}
}
