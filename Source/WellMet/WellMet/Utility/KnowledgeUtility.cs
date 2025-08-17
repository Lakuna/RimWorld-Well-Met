using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Lakuna.WellMet.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for checking whether information should be known.
	/// </summary>
	public static class KnowledgeUtility {
		/// <summary>
		/// The number of in-game ticks per in-game day.
		/// </summary>
		private const int TicksPerDay = 24 * 2500;

		/// <summary>
		/// The number of in-game ticks per in-game quadrum (in-game month/in-game season).
		/// </summary>
		private const int TicksPerQuadrum = 15 * TicksPerDay;

		/// <summary>
		/// The maximum amount of health that an unmodified human pawn can have.
		/// </summary>
		private const int HumanMaxHealth = 100;

		/// <summary>
		/// The amount of nutrition that an unmodified human pawn must consume daily.
		/// </summary>
		private const float HumanDailyNutrition = 1.6f;

		/// <summary>
		/// Check whether the given pawn is hostile to the player's faction.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given pawn is hostile to the player's faction.</returns>
		private static bool HostileToPlayer(Pawn pawn) => pawn != null && (pawn.HostileTo(Faction.OfPlayerSilentFail) || HostileToPlayer(pawn.Faction));

		/// <summary>
		/// Check whether the given faction is hostile to the player's faction.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether the given faction is hostile to the player's faction.</returns>
		private static bool HostileToPlayer(Faction faction) =>
			faction != null
			&& faction != Faction.OfPlayerSilentFail
			&& faction.RelationWith(Faction.OfPlayerSilentFail, true) != null
			&& faction.HostileTo(Faction.OfPlayerSilentFail);

		/// <summary>
		/// Get the type of the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The type of the pawn.</returns>
		public static PawnType TypeOf(Pawn pawn) =>
			(pawn == null) ? PawnType.Neutral
			: (MiscellaneousUtility.IsFreeNonSlaveColonist(pawn) || MiscellaneousUtility.IsAnimal(pawn) && pawn.Faction == Faction.OfPlayerSilentFail) ? PawnType.Colonist
			: MiscellaneousUtility.IsPlayerControlled(pawn) ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: MiscellaneousUtility.IsAnimal(pawn) ? PawnType.WildAnimal
			: HostileToPlayer(pawn) ? PawnType.Hostile
			: PawnType.Neutral;

		/// <summary>
		/// Get the type of the given faction.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>The type of the faction.</returns>
		public static PawnType TypeOf(Faction faction) =>
			(faction == null) ? PawnType.Neutral
			: (faction == Faction.OfPlayerSilentFail) ? PawnType.Colonist
			: HostileToPlayer(faction) ? PawnType.Hostile
			: PawnType.Neutral;

		/// <summary>
		/// Determines whether the given pawn is one that can be controlled by the player.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the pawn is player-controlled.</returns>
		public static bool IsPlayerControlled(Pawn pawn) => pawn != null && IsPlayerControlled(TypeOf(pawn), !pawn.Dead);

		/// <summary>
		/// Determines whether the given faction is one that can be controlled by the player.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether the faction is player-controlled.</returns>
		public static bool IsPlayerControlled(Faction faction) => faction != null && IsPlayerControlled(TypeOf(faction));

		/// <summary>
		/// Determines whether the given pawn type is one that can be controlled by the player.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <param name="isAlive">Whether the pawn is alive.</param>
		/// <returns>Whether the pawn type is player-controlled.</returns>
		public static bool IsPlayerControlled(PawnType type, bool isAlive = true) => (type == PawnType.Colonist || type == PawnType.Controlled) && isAlive;

		/// <summary>
		/// Determine whether or not the given pawn is a starting colonist. This can only be true while selecting starting colonists.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn is a starting colonist.</returns>
		private static bool IsStartingColonist(Pawn pawn) => pawn != null && (Find.GameInitData?.startingAndOptionalPawns?.Contains(pawn) ?? false);

		/// <summary>
		/// Determine whether or not the given pawn is related to any colonist.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn is related to any colonist.</returns>
		private static bool IsRelativeOfColonist(Pawn pawn) => pawn?.relations?.RelatedPawns?.Any((other) => TypeOf(other) == PawnType.Colonist) ?? false;

		/// <summary>
		/// Determine whether or not the given pawn's corpse was dead when the player discovered it.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn's corpse was dead when the player discovered it.</returns>
		public static bool IsAncientCorpse(Pawn pawn) => pawn != null && pawn.Dead && IsAncientCorpse(pawn.Corpse);

		/// <summary>
		/// Determine whether or not the given corpse was dead when the player discovered it.
		/// </summary>
		/// <param name="corpse">The corpse.</param>
		/// <returns>Whether or not the given corpse was dead when the player discovered it.</returns>
		public static bool IsAncientCorpse(Corpse corpse) => corpse != null && corpse.timeOfDeath <= 0;

		/// <summary>
		/// Determine whether the given information category is known for the given pawn.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <param name="isControl">Whether the obscured information is or contains an element that the player would use to control the pawn.</param>
		/// <returns>Whether the given information category is known for the given pawn.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, Pawn pawn, bool isControl = false) =>
			pawn == null
			|| (WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn)
				|| IsInformationKnownFor(category, TypeOf(pawn), isControl, !pawn.Dead)
				|| WellMetMod.Settings.AlwaysKnowMoreAboutColonistRelatives && IsRelativeOfColonist(pawn) && (category == InformationCategory.Backstory || category == InformationCategory.Basic || category == InformationCategory.Traits))
			&& !(!WellMetMod.Settings.LegacyMode && WellMetMod.Settings.HideAncientCorpses && IsAncientCorpse(pawn))
			&& !(!WellMetMod.Settings.LegacyMode && category == InformationCategory.Backstory && WellMetMod.Settings.BackstoryDiscoveryDifficulty * TicksPerQuadrum > TimeAsColonistOrPrisoner(pawn) && IsLearningEnabledFor(pawn)); // Backstory unlocked after one quadrum per difficulty.

		/// <summary>
		/// Determine whether the given information category is known for the given faction.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="faction">The faction.</param>
		/// <param name="isControl">Whether the obscured information is or contains an element that the player would use to control the faction.</param>
		/// <returns>Whether the given information category is known for the given faction.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, Faction faction, bool isControl = false) =>
			faction == null
			|| !WellMetMod.Settings.HideFactionInformation
			|| IsInformationKnownFor(category, TypeOf(faction), isControl);

		/// <summary>
		/// Determine whether the given information category is known for the given pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="type">The pawn type.</param>
		/// <param name="isControl">Whether the obscured information is or contains an element that the player would use to control the pawn or faction.</param>
		/// <param name="isAlive">Whether the pawn is alive.</param>
		/// <returns>Whether the given information category is known for the given pawn type.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, PawnType type, bool isControl = false, bool isAlive = true) =>
			WellMetMod.Settings.KnownInformation[(int)type, (int)category]
			|| WellMetMod.Settings.LegacyMode
			|| isControl && WellMetMod.Settings.NeverHideControls && IsPlayerControlled(type, isAlive);

		/// <summary>
		/// Determine whether the given information category is known for any pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether the given information category is known for any pawn type.</returns>
		public static bool IsInformationKnownForAny(InformationCategory category) => Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) => IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether or not learning is enabled for the given pawn. If learning is enabled, it may take time to learn a given piece of information; otherwise, information is always either known or not known.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not learning is enabled for the given pawn.</returns>
		public static bool IsLearningEnabledFor(Pawn pawn) => pawn != null && IsLearningEnabledFor(TypeOf(pawn));

		/// <summary>
		/// Determine whether or not learning is enabled for the given faction. If learning is enabled, it may take time to learn a given piece of information; otherwise, information is always either known or not known.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether or not learning is enabled for the given faction.</returns>
		public static bool IsLearningEnabledFor(Faction faction) => faction != null && IsLearningEnabledFor(TypeOf(faction));

		/// <summary>
		/// Determine whether or not learning is enabled for the given pawn type. If learning is enabled, it may take time to learn a given piece of information; otherwise, information is always either known or not known.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <returns>Whether or not learning is enabled for the given pawn type.</returns>
		public static bool IsLearningEnabledFor(PawnType type) => WellMetMod.Settings.LearningEnabled[(int)type] || WellMetMod.Settings.LegacyMode;

		/// <summary>
		/// Determine whether or not learning is enabled for any pawn type. If learning is enabled, it may take time to learn a given piece of information; otherwise, information is always either known or not known.
		/// </summary>
		/// <returns>Whether or not learning is enabled for any pawn type.</returns>
		public static bool IsLearningEnabledForAny() => Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) => IsLearningEnabledFor(type));

		/// <summary>
		/// Determine the number of ticks that the given pawn has spent as either a colonist, colony animal, or prisoner.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The number of ticks that the given pawn has spent as either a colonist, colony animal, or prisoner.</returns>
		private static float TimeAsColonistOrPrisoner(Pawn pawn) => (pawn?.records?.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) ?? 0) + (pawn?.records?.GetValue(RecordDefOf.TimeAsPrisoner) ?? 0);

		/// <summary>
		/// Get the trait definition of the wimp trait.
		/// </summary>
		/// <returns>The trait definition of the wimp trait.</returns>
		private static TraitDef TraitDefOfWimp() =>
#if V1_0 || V1_1 || V1_2
			null;
#else
			TraitDefOf.Wimp;
#endif

#if !(V1_0 || V1_1)
		/// <summary>
		/// Determine whether the given trait is known.
		/// </summary>
		/// <param name="trait">The trait.</param>
		/// <returns>Whether the given trait is known.</returns>
		public static bool IsTraitKnown(Trait trait) => trait == null || IsTraitKnown(trait.pawn, trait.def);
#endif

		/// <summary>
		/// Determine whether the given trait type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="trait">The trait type.</param>
		/// <returns>Whether the given trait type is known for the given pawn.</returns>
		public static bool IsTraitKnown(Pawn pawn, TraitDef trait) {
			// In vanilla RimWorld, `pawn == null` only during a growth moment. A better way to do this might be `Find.WindowStack.WindowOfType<Dialog_GrowthMomentChoices>() == null`.
			if (pawn == null) {
				return IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist) && (WellMetMod.Settings.AlwaysKnowGrowthMomentTraits || WellMetMod.Settings.TraitDiscoveryDifficulty <= 0);
			}

			if (!IsInformationKnownFor(InformationCategory.Traits, pawn)) {
				return false;
			}

			if (trait == null || WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn) || !IsLearningEnabledFor(pawn)) {
				return true;
			}

			// One trait per rarity (multiplicative inverse of commonality) per quadrum per difficulty.
			bool defaultUnlocked = TimeAsColonistOrPrisoner(pawn) > 1 / trait.GetGenderSpecificCommonality(pawn.gender) * TicksPerQuadrum * WellMetMod.Settings.TraitDiscoveryDifficulty;
			return !WellMetMod.Settings.EnableUniqueTraitUnlockConditions ? defaultUnlocked
				: trait == TraitDefOf.Bloodlust ? pawn.records.GetValue(RecordDefOf.Kills) >= WellMetMod.Settings.TraitDiscoveryDifficulty
				: trait == TraitDefOf.Pyromaniac ? pawn.records.GetValue(RecordDefOf.TimesInMentalState) >= WellMetMod.Settings.TraitDiscoveryDifficulty
				: trait == TraitDefOf.Brawler || trait.defName == "ShootingAccuracy" ? pawn.records.GetValue(RecordDefOf.ShotsFired) >= WellMetMod.Settings.TraitDiscoveryDifficulty * 10
				: trait == TraitDefOfWimp() || trait.defName == "Tough" || trait.defName == "Masochist" ? pawn.records.GetValue(RecordDefOf.DamageTaken) >= WellMetMod.Settings.TraitDiscoveryDifficulty * (HumanMaxHealth / 10)
				: trait == TraitDefOf.BodyPurist || trait == TraitDefOf.Transhumanist ? pawn.records.GetValue(RecordDefOf.OperationsReceived) >= WellMetMod.Settings.TraitDiscoveryDifficulty
				: trait.defName == "Gourmand" ? pawn.records.GetValue(RecordDefOf.NutritionEaten) >= WellMetMod.Settings.TraitDiscoveryDifficulty * HumanDailyNutrition * 10
				: defaultUnlocked;
		}

		/// <summary>
		/// Determine whether the given thought is known.
		/// </summary>
		/// <param name="thought">The thought.</param>
		/// <returns>Whether the given thought is known.</returns>
		public static bool IsThoughtKnown(Thought thought) => thought == null || IsThoughtKnown(thought.pawn, thought.def);

		/// <summary>
		/// Determine whether the given thought type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="thought">The thought type.</param>
		/// <returns>Whether the given thought type is known for the given pawn.</returns>
		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thought) =>
			IsInformationKnownFor(InformationCategory.Needs, pawn)
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				&& !(thought?.requiredGenes?.Count > 0 && !IsInformationKnownFor(InformationCategory.Advanced, pawn))
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				&& !(thought?.requiredHediffs?.Count > 0 && !IsInformationKnownFor(InformationCategory.Health, pawn))
#endif
				&& !(thought?.requiredTraits?.Any((traitDef) => !IsTraitKnown(pawn, traitDef)) ?? false);

		/// <summary>
		/// Determine whether the given skill is known.
		/// </summary>
		/// <param name="skill">The skill.</param>
		/// <returns>Whether the given skill is known.</returns>
		public static bool IsSkillKnown(SkillRecord skill) => skill == null || IsSkillKnown(MiscellaneousUtility.PawnOfSkillRecord(skill), skill.def);

		/// <summary>
		/// Determine whether the given skill type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="skill">The skill type.</param>
		/// <returns>Whether the given skill type is known for the given pawn.</returns>
		public static bool IsSkillKnown(Pawn pawn, SkillDef skill) {
			if (!IsInformationKnownFor(InformationCategory.Skills, pawn)) {
				return false;
			}

			if (pawn == null || skill == null || WellMetMod.Settings.SkillsDiscoveryDifficulty <= 0 || WellMetMod.Settings.AlwaysKnowStartingColonists && IsStartingColonist(pawn) || !IsLearningEnabledFor(pawn)) {
				return true;
			}

			IOrderedEnumerable<SkillRecord> skills = pawn.skills.skills.OrderByDescending((record) => record.Level);
			int order = skills.FirstIndexOf((record) => record.def == skill) + 1; // Learn the pawn's skills in order of their level (highest first).
			return TimeAsColonistOrPrisoner(pawn) > order * 5 * TicksPerDay * WellMetMod.Settings.SkillsDiscoveryDifficulty; // One skill per order per five days per difficulty.
		}
	}
}
