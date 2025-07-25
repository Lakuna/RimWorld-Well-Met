using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Utility {
	public static class KnowledgeUtility {
		private const int TicksPerQuadrum = 15 * 24 * 2500;

		private const int HumanMaxHealth = 100;

		private const float HumanDailyNutrition = 1.6f;

		public static readonly MethodInfo IsInformationKnownForPawnMethod = AccessTools.Method(typeof(KnowledgeUtility), nameof(IsInformationKnownFor), new Type[] { typeof(InformationCategory), typeof(Pawn) });

		public static readonly MethodInfo IsInformationKnownForFactionMethod = AccessTools.Method(typeof(KnowledgeUtility), nameof(IsInformationKnownFor), new Type[] { typeof(InformationCategory), typeof(Faction) });

		public static PawnType TypeOf(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: (pawn.IsFreeNonSlaveColonist || pawn.IsAnimal && pawn.Faction == Faction.OfPlayerSilentFail) ? PawnType.Colonist
			: pawn.IsPlayerControlled ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: (pawn.HostileTo(Faction.OfPlayerSilentFail) || pawn.Dead && pawn.Faction.HostileTo(Faction.OfPlayerSilentFail)) ? PawnType.Hostile
			: PawnType.Neutral;

		public static PawnType TypeOf(Faction faction) => faction == null
			? throw new ArgumentNullException(nameof(faction))
			: faction == Faction.OfPlayerSilentFail ? PawnType.Colonist
			: faction.HostileTo(Faction.OfPlayerSilentFail) ? PawnType.Hostile
			: PawnType.Neutral;

		private static bool IsStartingColonist(Pawn pawn) => Find.GameInitData?.startingAndOptionalPawns?.Contains(pawn) ?? false;

		public static bool IsInformationKnownFor(InformationCategory informationCategory, Pawn pawn) => WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn) || IsInformationKnownFor(informationCategory, TypeOf(pawn));

		public static bool IsInformationKnownFor(InformationCategory informationCategory, Faction faction) => !WellMetMod.Settings.HideFactionInformation || IsInformationKnownFor(informationCategory, TypeOf(faction));

		public static bool IsInformationKnownFor(InformationCategory informationCategory, PawnType pawnType) => WellMetMod.Settings.KnownInformation[(int)pawnType, (int)informationCategory];

		public static bool IsTraitKnown(Trait trait) => trait == null
			? throw new ArgumentNullException(nameof(trait))
			: IsTraitKnown(trait.pawn, trait.def);

		public static bool IsTraitKnown(Pawn pawn, TraitDef traitDef) {
			// In vanilla RimWorld, `pawn == null` only during a growth moment. A better way to do this might be `Find.WindowStack.WindowOfType<Dialog_GrowthMomentChoices>() == null`.
			if (pawn == null) {
				return IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist) && (WellMetMod.Settings.AlwaysKnowGrowthMomentTraits || WellMetMod.Settings.ColonistTraitDiscoveryDifficulty <= 0);
			}

			// Prevents some issues where some pawns cannot gain traits.
			if (traitDef == null) {
				return true;
			}

			if (!IsInformationKnownFor(InformationCategory.Traits, pawn)) {
				return false;
			}

			// Only colonists' traits might need to be learned over time.
			if (TypeOf(pawn) != PawnType.Colonist || WellMetMod.Settings.ColonistTraitDiscoveryDifficulty <= 0 || WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn)) {
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
			return pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) > 1 / traitDef.GetGenderSpecificCommonality(pawn.gender) * TicksPerQuadrum * WellMetMod.Settings.ColonistTraitDiscoveryDifficulty;
		}

		public static bool IsThoughtKnown(Thought thought) => thought == null
			? throw new ArgumentNullException(nameof(thought))
			: IsThoughtKnown(thought.pawn, thought.def);

		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thoughtDef) => thoughtDef == null
			? throw new ArgumentNullException(nameof(thoughtDef))
			: IsInformationKnownFor(InformationCategory.Needs, pawn) && !thoughtDef.requiredTraits.Any((traitDef) => !IsTraitKnown(pawn, traitDef));
	}
}
