﻿using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Lakuna.WellMet.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for checking whether information should be known.
	/// </summary>
	public static class KnowledgeUtility {
		/// <summary>
		/// The number of in-game ticks per quadrum (month/season).
		/// </summary>
		private const int TicksPerQuadrum = 15 * 24 * 2500;

		/// <summary>
		/// The maximum amount of health that an unmodified human pawn can have.
		/// </summary>
		private const int HumanMaxHealth = 100;

		/// <summary>
		/// The amount of nutrition that an unmodified human pawn must consume daily.
		/// </summary>
		private const float HumanDailyNutrition = 1.6f;

		/// <summary>
		/// Get the type of the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The type of the pawn.</returns>
		/// <exception cref="ArgumentNullException">When no pawn is given.</exception>
		public static PawnType TypeOf(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: (pawn.IsFreeNonSlaveColonist || pawn.IsAnimal && pawn.Faction == Faction.OfPlayerSilentFail) ? PawnType.Colonist
			: pawn.IsPlayerControlled ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: (pawn.HostileTo(Faction.OfPlayerSilentFail) || pawn.Dead && pawn.Faction.HostileTo(Faction.OfPlayerSilentFail)) ? PawnType.Hostile
			: PawnType.Neutral;

		/// <summary>
		/// Get the type of the given faction.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>The type of the faction.</returns>
		/// <exception cref="ArgumentNullException">When no faction is given.</exception>
		public static PawnType TypeOf(Faction faction) => faction == null
			? throw new ArgumentNullException(nameof(faction))
			: faction == Faction.OfPlayerSilentFail ? PawnType.Colonist
			: faction.HostileTo(Faction.OfPlayerSilentFail) ? PawnType.Hostile
			: PawnType.Neutral;

		/// <summary>
		/// Determine whether or not the given pawn is a starting colonist. This can only be true while selecting starting colonists.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn is a starting colonist.</returns>
		private static bool IsStartingColonist(Pawn pawn) => Find.GameInitData?.startingAndOptionalPawns?.Contains(pawn) ?? false;

		/// <summary>
		/// Determine whether the given information category is known for the given pawn.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given information category is known for the given pawn.</returns>
		public static bool IsInformationKnownFor(InformationCategory informationCategory, Pawn pawn) => WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn) || IsInformationKnownFor(informationCategory, TypeOf(pawn));

		/// <summary>
		/// Determine whether the given information category is known for the given faction.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether the given information category is known for the given faction.</returns>
		public static bool IsInformationKnownFor(InformationCategory informationCategory, Faction faction) => !WellMetMod.Settings.HideFactionInformation || IsInformationKnownFor(informationCategory, TypeOf(faction));

		/// <summary>
		/// Determine whether the given information category is known for the given pawn type.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="pawnType">The pawn type.</param>
		/// <returns>Whether the given information category is known for the given pawn type.</returns>
		public static bool IsInformationKnownFor(InformationCategory informationCategory, PawnType pawnType) => WellMetMod.Settings.KnownInformation[(int)pawnType, (int)informationCategory];

		/// <summary>
		/// Determine whether the given trait is known.
		/// </summary>
		/// <param name="trait">The trait.</param>
		/// <returns>Whether the given trait is known.</returns>
		/// <exception cref="ArgumentNullException">When no trait is given.</exception>
		public static bool IsTraitKnown(Trait trait) => trait == null
			? throw new ArgumentNullException(nameof(trait))
			: IsTraitKnown(trait.pawn, trait.def);

		/// <summary>
		/// Determine whether the given trait type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="traitDef">The trait type.</param>
		/// <returns>Whether the given trait type is known for the given pawn.</returns>
		public static bool IsTraitKnown(Pawn pawn, TraitDef traitDef) => pawn == null
			? IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist) && (WellMetMod.Settings.AlwaysKnowGrowthMomentTraits || WellMetMod.Settings.ColonistTraitDiscoveryDifficulty <= 0) // In vanilla RimWorld, `pawn == null` only during a growth moment. A better way to do this might be `Find.WindowStack.WindowOfType<Dialog_GrowthMomentChoices>() == null`.
			: traitDef == null // // Prevents some issues where some pawns cannot gain traits.
			|| IsInformationKnownFor(InformationCategory.Traits, pawn)
			&& (TypeOf(pawn) != PawnType.Colonist // Only colonists' traits might need to be learned over time.
				|| WellMetMod.Settings.ColonistTraitDiscoveryDifficulty <= 0
				|| WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn)
				|| (traitDef == TraitDefOf.Bloodlust ? pawn.records.GetValue(RecordDefOf.Kills) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty
					: traitDef == TraitDefOf.Pyromaniac ? pawn.records.GetValue(RecordDefOf.TimesInMentalState) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty
					: traitDef == TraitDefOf.Brawler || traitDef.defName == "ShootingAccuracy" ? pawn.records.GetValue(RecordDefOf.ShotsFired) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty * 10
					: traitDef == TraitDefOf.Wimp || traitDef.defName == "Tough" || traitDef.defName == "Masochist" ? pawn.records.GetValue(RecordDefOf.DamageTaken) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty * (HumanMaxHealth / 10)
					: traitDef == TraitDefOf.BodyPurist || traitDef == TraitDefOf.Transhumanist ? pawn.records.GetValue(RecordDefOf.OperationsReceived) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty
					: traitDef.defName == "Gourmand" ? pawn.records.GetValue(RecordDefOf.NutritionEaten) >= WellMetMod.Settings.ColonistTraitDiscoveryDifficulty * HumanDailyNutrition * 10
					: pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) > 1 / traitDef.GetGenderSpecificCommonality(pawn.gender) * TicksPerQuadrum * WellMetMod.Settings.ColonistTraitDiscoveryDifficulty));

		/// <summary>
		/// Determine whether the given thought is known.
		/// </summary>
		/// <param name="thought">The thought.</param>
		/// <returns>Whether the given thought is known.</returns>
		/// <exception cref="ArgumentNullException">When no thought is given.</exception>
		public static bool IsThoughtKnown(Thought thought) => thought == null
			? throw new ArgumentNullException(nameof(thought))
			: IsThoughtKnown(thought.pawn, thought.def);

		/// <summary>
		/// Determine whether the given thought type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="thoughtDef">The thought type.</param>
		/// <returns>Whether the given thought type is known for the given pawn.</returns>
		/// <exception cref="ArgumentNullException">When no thought type is given.</exception>
		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thoughtDef) => thoughtDef == null
			? throw new ArgumentNullException(nameof(thoughtDef))
			: IsInformationKnownFor(InformationCategory.Needs, pawn) && !thoughtDef.requiredTraits.Any((traitDef) => !IsTraitKnown(pawn, traitDef));
	}
}
