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
		/// The number of ticks per in-game hour.
		/// </summary>
		private const int TicksPerHour = 24;

		/// <summary>
		/// The number of ticks per in-game day.
		/// </summary>
		private const int TicksPerDay = 24 * TicksPerHour;

		/// <summary>
		/// The number of ticks per in-game quadrum (in-game month/in-game season).
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
		/// Determine whether the given information category is known for the given pawn.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <param name="isControl">Whether the obscured information is or contains an element that the player would use to control the pawn.</param>
		/// <returns>Whether the given information category is known for the given pawn.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, Pawn pawn, bool isControl = false) =>
			pawn == null
			|| (WellMetMod.Settings.AlwaysKnowStartingColonists && MiscellaneousUtility.IsStartingColonist(pawn)
				|| IsInformationKnownFor(category, MiscellaneousUtility.TypeOf(pawn), isControl, !pawn.Dead)
				|| WellMetMod.Settings.AlwaysKnowMoreAboutColonistRelatives && MiscellaneousUtility.IsRelativeOfColonist(pawn) && (category == InformationCategory.Backstory || category == InformationCategory.Basic || category == InformationCategory.Traits))
			&& !(!WellMetMod.Settings.LegacyMode && WellMetMod.Settings.HideAncientCorpses && MiscellaneousUtility.IsAncientCorpse(pawn))
			&& !(!WellMetMod.Settings.LegacyMode && category == InformationCategory.Backstory && WellMetMod.Settings.BackstoryDiscoveryDifficulty * TicksPerQuadrum > MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) && IsLearningEnabledFor(pawn)); // Backstory unlocked after one quadrum per difficulty.

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
			|| IsInformationKnownFor(category, MiscellaneousUtility.TypeOf(faction), isControl);

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
			|| isControl && WellMetMod.Settings.NeverHideControls && MiscellaneousUtility.IsPlayerControlled(type, isAlive);

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
		public static bool IsLearningEnabledFor(Pawn pawn) => pawn != null && IsLearningEnabledFor(MiscellaneousUtility.TypeOf(pawn));

		/// <summary>
		/// Determine whether or not learning is enabled for the given faction. If learning is enabled, it may take time to learn a given piece of information; otherwise, information is always either known or not known.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether or not learning is enabled for the given faction.</returns>
		public static bool IsLearningEnabledFor(Faction faction) => faction != null && IsLearningEnabledFor(MiscellaneousUtility.TypeOf(faction));

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

			if (trait == null || WellMetMod.Settings.AlwaysKnowStartingColonists && MiscellaneousUtility.IsStartingColonist(pawn) || !IsLearningEnabledFor(pawn)) {
				return true;
			}

			// One trait per rarity (multiplicative inverse of commonality) per quadrum per difficulty.
			bool defaultUnlocked = MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > 1 / trait.GetGenderSpecificCommonality(pawn.gender) * TicksPerQuadrum * WellMetMod.Settings.TraitDiscoveryDifficulty;
			return (!WellMetMod.Settings.EnableUniqueTraitUnlockConditions || WellMetMod.Settings.LegacyMode) ? defaultUnlocked
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

			if (pawn == null || skill == null || WellMetMod.Settings.SkillsDiscoveryDifficulty <= 0 || WellMetMod.Settings.AlwaysKnowStartingColonists && MiscellaneousUtility.IsStartingColonist(pawn) || !IsLearningEnabledFor(pawn)) {
				return true;
			}

			IOrderedEnumerable<SkillRecord> skills = pawn.skills.skills.OrderByDescending((record) => record.Level);
			int order = skills.FirstIndexOf((record) => record.def == skill) + 1; // Learn the pawn's skills in order of their level (highest first).
			return MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > order * 5 * TicksPerDay * WellMetMod.Settings.SkillsDiscoveryDifficulty; // One skill per order per five days per difficulty.
		}
	}
}
