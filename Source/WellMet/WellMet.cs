using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WellMet {
	public class WellMet : Mod {
		private const int TicksPerDay = 60000;
		public const string UnknownTraitName = "???";
		public const string UnknownTraitDescription = "You have to get to know this pawn better in order to discover this trait.";
		public const string UnknownThoughtName = "Thought from unknown trait";
		public const string UnknownThoughtDescription = "You have to get to know this pawn better in order to discover what is causing this thought.";

		public static WellMet Instance;
		public static Func<Trait, bool> defaultDiscoveryCondition = (trait) =>
			trait.pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) >
			trait.def.GetGenderSpecificCommonality(trait.pawn.gender) * (TicksPerDay * WellMet.Instance.settings.daysToUnlockPerCommonality);
		private static Dictionary<TraitDef, Func<Trait, bool>> customDiscoveryConditions;
		public static Dictionary<TraitDef, Func<Trait, bool>> CustomDiscoveryConditions {
			get {
				if (WellMet.customDiscoveryConditions == null) {
					WellMet.customDiscoveryConditions = new Dictionary<TraitDef, Func<Trait, bool>> {
						{ TraitDefOf.Bloodlust, (trait) => trait.pawn.records.GetValue(RecordDefOf.Kills) > 0 },
						{ TraitDefOf.Brawler, (trait) => trait.pawn.records.GetValue(RecordDefOf.ShotsFired) > 0 },
						{ TraitDefOf.Masochist, (trait) => trait.pawn.records.GetValue(RecordDefOf.DamageTaken) > 0 },
						{ TraitDefOf.Tough, (trait) => trait.pawn.records.GetValue(RecordDefOf.DamageTaken) > 0 },
						{ TraitDefOf.Wimp, (trait) => trait.pawn.records.GetValue(RecordDefOf.DamageTaken) > 0 },
						{ TraitDefOf.Transhumanist, (trait) => trait.pawn.records.GetValue(RecordDefOf.OperationsReceived) > 0 },
						{ TraitDefOf.BodyPurist, (trait) => trait.pawn.records.GetValue(RecordDefOf.OperationsReceived) > 0 },
						{ TraitDefOf.Pyromaniac, (trait) => trait.pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 0 }
					};
				}

				return WellMet.customDiscoveryConditions;
			}
		}

		public static bool TraitIsDiscoveredForPawn(Trait trait) {
			_ = WellMet.CustomDiscoveryConditions.TryGetValue(trait.def, out Func<Trait, bool> customDiscoveryCondition);

			return customDiscoveryCondition != null
				? customDiscoveryCondition(trait)
				: WellMet.defaultDiscoveryCondition(trait);
		}

		public static bool ThoughtIsHiddenForPawn(Pawn pawn, ThoughtDef thoughtDef) => !thoughtDef.requiredTraits.NullOrEmpty()
			&& thoughtDef.requiredTraits.Any((traitDef) => pawn.story.traits.HasTrait(traitDef)
			&& WellMet.TraitIsDiscoveredForPawn(pawn.story.traits.GetTrait(traitDef)));

		public Settings settings;

		public WellMet(ModContentPack content) : base(content) {
			if (WellMet.Instance == null) {
				WellMet.Instance = this;
			}

			this.settings = this.GetSettings<Settings>();
		}

		public override void DoSettingsWindowContents(Rect rect) {
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(rect);
			_ = listingStandard.Label("Number of days before trait unlock per commonality (" + this.settings.daysToUnlockPerCommonality + ")");
			this.settings.daysToUnlockPerCommonality = listingStandard.Slider(this.settings.daysToUnlockPerCommonality, 0, 60);
			listingStandard.End();

			base.DoSettingsWindowContents(rect);
		}

		public override string SettingsCategory() => "Well Met";
	}
}
