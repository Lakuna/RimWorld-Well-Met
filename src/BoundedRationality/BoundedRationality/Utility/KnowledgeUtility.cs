using System;
using System.Linq;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for checking whether information should be known.
	/// </summary>
	public static class KnowledgeUtility {
		/// <summary>
		/// The number of ticks per in-game hour.
		/// </summary>
		private const int TicksPerHour = 2500;

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
		/// Determine whether or not the given information category represents unchanging information.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether or not the given information category represents unchanging information.</returns>
		public static bool IsStatic(InformationCategory category) =>
			category == InformationCategory.Backstory
			|| category == InformationCategory.Basic
			|| category == InformationCategory.Traits;

		/// <summary>
		/// Determine whether the given information category is known for the given thing (pawn).
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="thing">The thing (pawn).</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>Whether the given information category is known for the given thing (pawn).</returns>
		public static bool IsInformationKnownFor(InformationCategory category, Thing thing, ControlCategory controlCategory = ControlCategory.Default) =>
			thing is null
			|| thing is Pawn pawn && IsInformationKnownFor(category, pawn, controlCategory)
			|| thing is Corpse corpse && IsInformationKnownFor(category, corpse.InnerPawn, controlCategory);

		/// <summary>
		/// Determine whether the given information category is known for the given pawn.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>Whether the given information category is known for the given pawn.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, Pawn pawn, ControlCategory controlCategory = ControlCategory.Default) =>
			pawn is null
			|| (BoundedRationalityMod.Settings.AlwaysKnowStartingColonists && MiscellaneousUtility.IsStartingColonist(pawn)
				|| IsInformationKnownFor(category, MiscellaneousUtility.TypeOf(pawn), controlCategory, !pawn.Dead)
				|| BoundedRationalityMod.Settings.AlwaysKnowMoreAboutColonistRelatives && MiscellaneousUtility.IsRelativeOfColonist(pawn) && IsStatic(category))
			&& !(!BoundedRationalityMod.Settings.LegacyMode
				&& BoundedRationalityMod.Settings.HideAncientCorpses && MiscellaneousUtility.IsAncientCorpse(pawn));

		/// <summary>
		/// Determine whether the given information category is known for the given faction.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="faction">The faction.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>Whether the given information category is known for the given faction.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, Faction faction, ControlCategory controlCategory = ControlCategory.Default) =>
			faction is null
			|| !BoundedRationalityMod.Settings.HideFactionInformation
			|| IsInformationKnownFor(category, MiscellaneousUtility.TypeOf(faction), controlCategory);

		/// <summary>
		/// Determine whether the given information category is known for the given pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="type">The pawn type.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <param name="isAlive">Whether the pawn is alive.</param>
		/// <returns>Whether the given information category is known for the given pawn type.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, PawnType type, ControlCategory controlCategory = ControlCategory.Default, bool isAlive = true) =>
			BoundedRationalityMod.Settings.KnownInformation[(int)type, (int)category]
			|| BoundedRationalityMod.Settings.LegacyMode
			|| controlCategory == ControlCategory.Control && BoundedRationalityMod.Settings.NeverHideControls && MiscellaneousUtility.IsPlayerControlled(type, isAlive)
			|| controlCategory == ControlCategory.Letter && BoundedRationalityMod.Settings.NeverHideLetters
			|| controlCategory == ControlCategory.Message && BoundedRationalityMod.Settings.NeverHideMessages
			|| controlCategory == ControlCategory.TextMote && BoundedRationalityMod.Settings.NeverHideTextMotes
			|| controlCategory == ControlCategory.Alert && BoundedRationalityMod.Settings.NeverHideAlerts
			|| category == InformationCategory.Traits && type == PawnType.Colonist && BoundedRationalityMod.Settings.AlwaysKnowGrowthMomentTraits && MiscellaneousUtility.IsInGrowthMoment();

		/// <summary>
		/// Determine whether the given information category is known for any pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether the given information category is known for any pawn type.</returns>
		public static bool IsInformationKnownForAny(InformationCategory category) =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) =>
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether the given information category is known for all pawn types.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether the given information category is known for all pawn types.</returns>
		public static bool IsInformationKnownForAll(InformationCategory category) =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().All((type) =>
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether any information category is known for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <returns>Whether any information category is known for the given pawn type.</returns>
		public static bool IsAnyInformationKnownFor(PawnType type) =>
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().Any((category) =>
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether all information categories are known for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <returns>Whether all information categories are known for the given pawn type.</returns>
		public static bool IsAllInformationKnownFor(PawnType type) =>
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().All((category) =>
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether all information is known for all pawn types.
		/// </summary>
		/// <returns>Whether all information is known for all pawn types.</returns>
		public static bool IsAllInformationKnownForAll() =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().All((type) =>
				IsAllInformationKnownFor(type));

		/// <summary>
		/// Determine whether or not the information known about one pawn type contains all of the information known about another pawn type.
		/// </summary>
		/// <param name="superset">The pawn type to check as a superset of the other.</param>
		/// <param name="subset">The pawn type to check as a subset of the other.</param>
		/// <returns>Whether or not `superset` is a superset of `subset`.</returns>
		public static bool IsInformationSupersetOf(PawnType superset, PawnType subset) =>
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().All((category) =>
				IsInformationKnownFor(category, superset) || !IsInformationKnownFor(category, subset));

		/// <summary>
		/// Determine whether or not the information known about one pawn type contains all of the information known about any other pawn type.
		/// </summary>
		/// <param name="superset">The pawn type to check as a superset of the other.</param>
		/// <returns>Whether or not `superset` is a superset of every other pawn type.</returns>
		public static bool IsInformationSupersetOfAny(PawnType superset) =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((subset) =>
				IsInformationSupersetOf(superset, subset));

		/// <summary>
		/// Determine whether or not learning is enabled for the given pawn and information category pair.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether or not learning is enabled for the given pawn and information category pair.</returns>
		public static bool IsLearningEnabledFor(InformationCategory category, Pawn pawn, bool ignoreDifficulty = false) =>
			IsInformationKnownFor(category, pawn)
			&& IsLearningEnabledFor(category, MiscellaneousUtility.TypeOf(pawn), ignoreDifficulty)
			&& !(BoundedRationalityMod.Settings.AlwaysKnowStartingColonists && MiscellaneousUtility.IsStartingColonist(pawn))
			&& !(category == InformationCategory.Traits && BoundedRationalityMod.Settings.AlwaysKnowGrowthMomentTraits && (pawn is null || MiscellaneousUtility.IsInGrowthMoment()))
			&& !(IsStatic(category) && BoundedRationalityMod.Settings.AlwaysKnowMoreAboutColonistRelatives && MiscellaneousUtility.IsRelativeOfColonist(pawn));

		/// <summary>
		/// Determine whether or not learning is enabled for the given faction and information category pair.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="faction">The faction.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether or not learning is enabled for the given faction and information category pair.</returns>
		public static bool IsLearningEnabledFor(InformationCategory category, Faction faction, bool ignoreDifficulty = false) =>
			IsInformationKnownFor(category, faction)
			&& IsLearningEnabledFor(category, MiscellaneousUtility.TypeOf(faction), ignoreDifficulty);

		/// <summary>
		/// Determine whether or not learning is enabled for the given pawn type and information category pair.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="type">The pawn type.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether or not learning is enabled for the given pawn type and information category pair.</returns>
		public static bool IsLearningEnabledFor(InformationCategory category, PawnType type, bool ignoreDifficulty = false) {
			if (!IsInformationKnownFor(category, type)) {
				return false;
			}

			bool learningEnabled = BoundedRationalityMod.Settings.LearningEnabled[(int)type];
			switch (category) {
				case InformationCategory.Backstory:
					return !BoundedRationalityMod.Settings.LegacyMode && (BoundedRationalityMod.Settings.BackstoryLearningDifficulty > 0 || ignoreDifficulty) && learningEnabled;
				case InformationCategory.Skills:
					return !BoundedRationalityMod.Settings.LegacyMode && (BoundedRationalityMod.Settings.SkillsLearningDifficulty > 0 || ignoreDifficulty) && learningEnabled;
				case InformationCategory.Traits:
					return (BoundedRationalityMod.Settings.TraitsLearningDifficulty > 0 || ignoreDifficulty) && (BoundedRationalityMod.Settings.LegacyMode || learningEnabled);
				default:
					return false;
			}
		}

		/// <summary>
		/// Determine whether learning the given information category is enabled for any pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning the given information category is enabled for any pawn type.</returns>
		public static bool IsLearningEnabledForAny(InformationCategory category, bool ignoreDifficulty = false) =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) =>
				IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether learning the given information category is enabled for all pawn types.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning the given information category is enabled for all pawn types.</returns>
		public static bool IsLearningEnabledForAll(InformationCategory category, bool ignoreDifficulty = false) =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().All((type) =>
				IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether learning any information category is enabled for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning any information category is enabled for the given pawn type.</returns>
		public static bool IsAnyLearningEnabledFor(PawnType type, bool ignoreDifficulty = false) =>
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().Any((category) =>
				IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether learning any information category is enabled for any pawn type.
		/// </summary>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning the given information category is enabled for any pawn type.</returns>
		public static bool IsAnyLearningEnabledForAny(bool ignoreDifficulty = false) =>
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) =>
				IsAnyLearningEnabledFor(type, ignoreDifficulty));

		/// <summary>
		/// Determine whether learning all applicable information categories is enabled for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning all applicable information categories is enabled for the given pawn type.</returns>
		public static bool IsAllLearningEnabledFor(PawnType type, bool ignoreDifficulty = false) =>
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>()
				.Where((category) => category == InformationCategory.Backstory
					|| category == InformationCategory.Skills
					|| category == InformationCategory.Traits) // Filter to only information categories for which learning has been implemented.
				.All((category) => IsLearningEnabledFor(category, type, ignoreDifficulty));

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

		/// <summary>
		/// Determine whether the given trait is known.
		/// </summary>
		/// <param name="trait">The trait.</param>
		/// <returns>Whether the given trait is known.</returns>
		public static bool IsTraitKnown(Trait trait) =>
			trait is null
#if V1_0 || V1_1
			|| IsTraitKnown(null, trait.def);
#else
			|| IsTraitKnown(trait.pawn, trait.def);
#endif

		/// <summary>
		/// Determine whether the given trait type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="trait">The trait type.</param>
		/// <returns>Whether the given trait type is known for the given pawn.</returns>
		public static bool IsTraitKnown(Pawn pawn, TraitDef trait) {
			// In vanilla RimWorld, `pawn is null` only during a growth moment.
			if (pawn is null || MiscellaneousUtility.IsInGrowthMoment()) {
				return IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist) && !IsLearningEnabledFor(InformationCategory.Traits, pawn);
			}

			if (!IsInformationKnownFor(InformationCategory.Traits, pawn)) {
				return false;
			}

			if (trait is null || !IsLearningEnabledFor(InformationCategory.Traits, pawn)) {
				return true;
			}

			// One trait per rarity (multiplicative inverse of commonality) per quadrum per difficulty.
			bool defaultUnlocked = MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > 1 / trait.GetGenderSpecificCommonality(pawn.gender) * TicksPerQuadrum * BoundedRationalityMod.Settings.TraitsLearningDifficulty;
			return (!BoundedRationalityMod.Settings.EnableUniqueTraitUnlockConditions || BoundedRationalityMod.Settings.LegacyMode) ? defaultUnlocked
				: trait == TraitDefOf.Bloodlust ? pawn.records.GetValue(RecordDefOf.Kills) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty
				: trait == TraitDefOf.Pyromaniac ? pawn.records.GetValue(RecordDefOf.TimesInMentalState) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty
				: trait == TraitDefOf.Brawler || trait.defName == "ShootingAccuracy" ? pawn.records.GetValue(RecordDefOf.ShotsFired) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty * 10
				: trait == TraitDefOfWimp() || trait.defName == "Tough" || trait.defName == "Masochist" ? pawn.records.GetValue(RecordDefOf.DamageTaken) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty * (HumanMaxHealth / 10)
				: trait == TraitDefOf.BodyPurist || trait == TraitDefOf.Transhumanist ? pawn.records.GetValue(RecordDefOf.OperationsReceived) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty
				: trait.defName == "Gourmand" ? pawn.records.GetValue(RecordDefOf.NutritionEaten) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty * HumanDailyNutrition * 10
				: defaultUnlocked;
		}

		/// <summary>
		/// Determine whether the given thought is known.
		/// </summary>
		/// <param name="thought">The thought.</param>
		/// <returns>Whether the given thought is known.</returns>
		public static bool IsThoughtKnown(Thought thought) =>
			thought is null
			|| IsThoughtKnown(thought.pawn, thought.def);

		/// <summary>
		/// Determine whether the given thought type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="thought">The thought type.</param>
		/// <returns>Whether the given thought type is known for the given pawn.</returns>
		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thought) =>
			IsInformationKnownFor(InformationCategory.Needs, pawn)
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				&& !(thought?.requiredGenes?.Count > 0 && !IsInformationKnownFor(InformationCategory.Personal, pawn))
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
		public static bool IsSkillKnown(SkillRecord skill) =>
			skill is null
			|| IsSkillKnown(MiscellaneousUtility.PawnOfSkillRecord(skill), skill.def);

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

			if (pawn is null || skill is null || !IsLearningEnabledFor(InformationCategory.Skills, pawn)) {
				return true;
			}

			IOrderedEnumerable<SkillRecord> skills = pawn.skills.skills.OrderByDescending((record) => record.Level);
			int order = skills.FirstIndexOf((record) => record.def == skill) + 1; // Learn the pawn's skills in order of their level (highest first).
			return MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > order * 5 * TicksPerDay * BoundedRationalityMod.Settings.SkillsLearningDifficulty; // One skill per order per five days per difficulty.
		}

		/// <summary>
		/// Determine whether the given backstory is known for the given pawn.
		/// </summary>
		/// <param name="backstory">The backstory.</param>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given backstory is known for the given pawn.</returns>
		public static bool IsBackstoryKnown(
#if V1_0 || V1_1 || V1_2 || V1_3
			Backstory backstory,
#else
			BackstoryDef backstory,
#endif
			Pawn pawn) => backstory is null || IsBackstoryKnown(backstory.slot, pawn);

		/// <summary>
		/// Determine whether the given backstory is known for the given pawn.
		/// </summary>
		/// <param name="backstory">The backstory.</param>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given backstory is known for the given pawn.</returns>
		public static bool IsBackstoryKnown(BackstorySlot backstory, Pawn pawn) {
			if (!IsInformationKnownFor(InformationCategory.Backstory, pawn)) {
				return false;
			}

			if (pawn is null || !IsLearningEnabledFor(InformationCategory.Backstory, pawn)) {
				return true;
			}

			int order = backstory == BackstorySlot.Adulthood ? 1 : 2;
			return MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > order * TicksPerQuadrum * BoundedRationalityMod.Settings.BackstoryLearningDifficulty; // Backstory unlocked after one slot per quadrum per difficulty (more recent slots first).
		}
	}
}
