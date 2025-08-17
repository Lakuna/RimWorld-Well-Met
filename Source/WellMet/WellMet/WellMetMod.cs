using Lakuna.WellMet.Utility;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetMod : Mod {
		private const float CheckboxSize = 24;

		private const float ScrollViewMargin = 18; // Reduce the scroll view by this width so that the vertical scroll bar doesn't overflow the window horizontally.

		private const float ExtraScrollHeight = 250; // Must be larger than the maximum height that can be added to the UI in one frame.

		internal static WellMetSettings Settings { get; private set; }

		public WellMetMod(ModContentPack content) : base(content) {
			Settings = this.GetSettings<WellMetSettings>();
			this.settingsScrollPosition = Vector2.zero;
			this.totalSettingsHeight = 99999; // Arbitrarily large number.
		}

		private Vector2 settingsScrollPosition;

		private float totalSettingsHeight;

		public override void DoSettingsWindowContents(Rect inRect) {
			base.DoSettingsWindowContents(inRect);

			Rect scrollViewRect = new Rect(0, 0, inRect.width - ScrollViewMargin, this.totalSettingsHeight + ExtraScrollHeight);
			Widgets.BeginScrollView(inRect, ref this.settingsScrollPosition, scrollViewRect);

			PawnType[] pawnTypes = Enum.GetValues(typeof(PawnType)).OfType<PawnType>().ToArray();
			InformationCategory[] informationCategories = Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().ToArray();

			int rowCount = informationCategories.Length + 1; // Add one blank row for the column labels.
			float labelHeight = Text.LineHeight;
			float rowHeight = Math.Max(labelHeight, CheckboxSize);
			Rect tableRect = new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width, Settings.LegacyMode ? 0 : rowHeight * rowCount);
			Rect learningEnabledLabelRect = new Rect(scrollViewRect.x, tableRect.yMax, scrollViewRect.width, Settings.LegacyMode ? 0 : labelHeight);
			Rect learningEnabledRect = new Rect(tableRect.x, learningEnabledLabelRect.yMax, tableRect.width, Settings.LegacyMode ? 0 : rowHeight); // Horizontally align the "learning enabled" rectangle with the table so that the column labels can be reused.

			if (!Settings.LegacyMode) {
				int columnCount = pawnTypes.Length + 1; // Add one blank column for the row labels.
				float columnWidth = tableRect.width / columnCount;

				// Draw column labels.
				for (int i = 0; i < pawnTypes.Length; i++) {
					Rect columnRect = new Rect(tableRect.x + columnWidth * (i + 1), tableRect.y, columnWidth, tableRect.height); // Add one blank column for the row labels.
					Rect labelRect = new Rect(columnRect.x, columnRect.y, columnRect.width, rowHeight);
					Widgets.Label(labelRect, pawnTypes[i].ToString().Translate().CapitalizeFirst());

					// Draw label tooltips.
					if (Mouse.IsOver(labelRect)) {
						TooltipHandler.TipRegion(labelRect, MiscellaneousUtility.EndWithPeriod((pawnTypes[i].ToString() + "Blurb").Translate().CapitalizeFirst()));
					}
				}

				// Draw rows.
				for (int i = 0; i < informationCategories.Length; i++) {
					// Draw row label.
					Rect rowRect = new Rect(tableRect.x, tableRect.y + rowHeight * (i + 1), tableRect.width, rowHeight); // Add one blank row for the column labels.
					Rect labelRect = new Rect(rowRect.x, rowRect.y, columnWidth, rowRect.height);
					Widgets.Label(labelRect, informationCategories[i].ToString().Translate().CapitalizeFirst());

					// Draw label tooltips.
					if (Mouse.IsOver(labelRect)) {
						TooltipHandler.TipRegion(labelRect, MiscellaneousUtility.EndWithPeriod((informationCategories[i].ToString() + "Blurb").Translate().CapitalizeFirst()));
					}

					// Draw checkboxes.
					for (int j = 0; j < pawnTypes.Length; j++) {
						Rect checkboxRect = new Rect(rowRect.x + columnWidth * (j + 1), rowRect.y, columnWidth, rowRect.height); // Add one blank column for the row labels.

						bool value = KnowledgeUtility.IsInformationKnownFor(informationCategories[i], pawnTypes[j]);
						Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
						Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]] = value;

						// Draw checkbox tooltips.
						if (Mouse.IsOver(checkboxRect)) {
							TooltipHandler.TipRegion(checkboxRect, pawnTypes[j].ToString().Translate().CapitalizeFirst() + ": " + informationCategories[i].ToString().Translate());
						}
					}
				}

				// Draw "learning enabled" label.
				Widgets.Label(learningEnabledLabelRect, "LearningEnabled".Translate().CapitalizeFirst());

				// Draw "learning enabled" row.
				for (int i = 0; i < pawnTypes.Length; i++) {
					Rect checkboxRect = new Rect(learningEnabledRect.x + columnWidth * (i + 1), learningEnabledRect.y, columnWidth, learningEnabledRect.height); // Add one blank column for the row labels.

					bool value = KnowledgeUtility.IsLearningEnabledFor(pawnTypes[i]);
					Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
					Settings.LearningEnabled[(int)pawnTypes[i]] = value;
				}
			}

			Rect listingRect = new Rect(scrollViewRect.x, learningEnabledRect.yMax, scrollViewRect.width, scrollViewRect.height - tableRect.height - learningEnabledLabelRect.height - learningEnabledRect.height);
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(listingRect);

			bool legacyMode = Settings.LegacyMode;
			listing.CheckboxLabeled("LegacyMode".Translate().CapitalizeFirst(), ref legacyMode);
			Settings.LegacyMode = legacyMode;

			if (!Settings.LegacyMode) {
				bool hideFactionInformation = Settings.HideFactionInformation;
				listing.CheckboxLabeled("HideFactionInformation".Translate().CapitalizeFirst(), ref hideFactionInformation);
				Settings.HideFactionInformation = hideFactionInformation;

				bool neverHideControls = Settings.NeverHideControls;
				listing.CheckboxLabeled("NeverHideControls".Translate().CapitalizeFirst(), ref neverHideControls);
				Settings.NeverHideControls = neverHideControls;

				bool hideAncientCorpses = Settings.HideAncientCorpses;
				listing.CheckboxLabeled("HideAncientCorpses".Translate().CapitalizeFirst(), ref hideAncientCorpses);
				Settings.HideAncientCorpses = hideAncientCorpses;

				bool alwaysKnowMoreAboutColonistRelatives = Settings.AlwaysKnowMoreAboutColonistRelatives;
				listing.CheckboxLabeled("AlwaysKnowMoreAboutColonistRelatives".Translate().CapitalizeFirst(), ref alwaysKnowMoreAboutColonistRelatives);
				Settings.AlwaysKnowMoreAboutColonistRelatives = alwaysKnowMoreAboutColonistRelatives;
			}

			bool alwaysKnowStartingColonists = Settings.AlwaysKnowStartingColonists;
			listing.CheckboxLabeled("AlwaysKnowStartingColonists".Translate().CapitalizeFirst(), ref alwaysKnowStartingColonists);
			Settings.AlwaysKnowStartingColonists = alwaysKnowStartingColonists;

			if (!KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, PawnType.Colonist) && !Settings.AlwaysKnowStartingColonists && !Settings.NeverHideControls) {
#if V1_0
				listing.Label(MiscellaneousUtility.EndWithPeriod("WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst()));
#elif V1_1 || V1_2
				_ = listing.Label(MiscellaneousUtility.EndWithPeriod("WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst()).Resolve().Colorize(ColoredText.WarningColor));
#else
				_ = listing.Label(MiscellaneousUtility.EndWithPeriod("WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst()).Colorize(ColoredText.WarningColor));
#endif
			}

			if (pawnTypes.Any((type) => KnowledgeUtility.IsLearningEnabledFor(type) && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, type))) {
#if V1_0 || V1_1 || V1_2 || V1_3
				listing.Label("TraitDiscoveryDifficulty".Translate(Settings.TraitDiscoveryDifficulty).CapitalizeFirst());
				Settings.TraitDiscoveryDifficulty = (int)listing.Slider(Settings.TraitDiscoveryDifficulty, 0, 10);
#else
				Settings.TraitDiscoveryDifficulty = (int)listing.SliderLabeled("TraitDiscoveryDifficulty".Translate(Settings.TraitDiscoveryDifficulty).CapitalizeFirst(), Settings.TraitDiscoveryDifficulty, 0, 10);
#endif

				if (Settings.TraitDiscoveryDifficulty > 0) {
					if (KnowledgeUtility.IsLearningEnabledFor(PawnType.Colonist)) {
						bool alwaysKnowGrowthMoments = Settings.AlwaysKnowGrowthMomentTraits;
						listing.CheckboxLabeled("AlwaysKnowGrowthMomentTraits".Translate().CapitalizeFirst(), ref alwaysKnowGrowthMoments);
						Settings.AlwaysKnowGrowthMomentTraits = alwaysKnowGrowthMoments;
					}

					if (!Settings.LegacyMode) {
						bool enableUniqueTraitUnlockConditions = Settings.EnableUniqueTraitUnlockConditions;
						listing.CheckboxLabeled("EnableUniqueTraitUnlockConditions".Translate().CapitalizeFirst(), ref enableUniqueTraitUnlockConditions);
						Settings.EnableUniqueTraitUnlockConditions = enableUniqueTraitUnlockConditions;
					}
				}
			}

			if (!Settings.LegacyMode && pawnTypes.Any((type) => KnowledgeUtility.IsLearningEnabledFor(type) && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, type))) {
#if V1_0 || V1_1 || V1_2 || V1_3
				listing.Label("BackstoryDiscoveryDifficulty".Translate(Settings.BackstoryDiscoveryDifficulty).CapitalizeFirst());
				Settings.BackstoryDiscoveryDifficulty = (int)listing.Slider(Settings.BackstoryDiscoveryDifficulty, 0, 10);
#else
				Settings.BackstoryDiscoveryDifficulty = (int)listing.SliderLabeled("BackstoryDiscoveryDifficulty".Translate(Settings.BackstoryDiscoveryDifficulty).CapitalizeFirst(), Settings.BackstoryDiscoveryDifficulty, 0, 10);
#endif
			}

			if (!Settings.LegacyMode && pawnTypes.Any((type) => KnowledgeUtility.IsLearningEnabledFor(type) && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, type))) {
#if V1_0 || V1_1 || V1_2 || V1_3
				listing.Label("SkillsDiscoveryDifficulty".Translate(Settings.SkillsDiscoveryDifficulty).CapitalizeFirst());
				Settings.SkillsDiscoveryDifficulty = (int)listing.Slider(Settings.SkillsDiscoveryDifficulty, 0, 10);
#else
				Settings.SkillsDiscoveryDifficulty = (int)listing.SliderLabeled("SkillsDiscoveryDifficulty".Translate(Settings.SkillsDiscoveryDifficulty).CapitalizeFirst(), Settings.SkillsDiscoveryDifficulty, 0, 10);
#endif
			}

			listing.End();
			this.totalSettingsHeight = listingRect.y + listing.CurHeight;
			Widgets.EndScrollView();
		}

		public override string SettingsCategory() => "WellMet".Translate().CapitalizeFirst();
	}
}
