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
					Widgets.Label(labelRect, $"BR.{pawnTypes[i]}".Translate().CapitalizeFirst());

					// Draw label tooltips.
					if (Mouse.IsOver(labelRect)) {
						TooltipHandler.TipRegion(labelRect, MiscellaneousUtility.EndWithPeriod($"BR.{pawnTypes[i]}.Desc".Translate().CapitalizeFirst()));
					}
				}

				// Draw rows.
				for (int i = 0; i < informationCategories.Length; i++) {
					// Draw row label.
					Rect rowRect = new Rect(tableRect.x, tableRect.y + rowHeight * (i + 1), tableRect.width, rowHeight); // Add one blank row for the column labels.
					Rect labelRect = new Rect(rowRect.x, rowRect.y, columnWidth, rowRect.height);
					Widgets.Label(labelRect, $"BR.{informationCategories[i]}".Translate().CapitalizeFirst());

					// Draw label tooltips.
					if (Mouse.IsOver(labelRect)) {
						TooltipHandler.TipRegion(labelRect, MiscellaneousUtility.EndWithPeriod($"BR.{informationCategories[i]}.Desc".Translate().CapitalizeFirst()));
					}

					// Draw checkboxes.
					for (int j = 0; j < pawnTypes.Length; j++) {
						Rect checkboxRect = new Rect(rowRect.x + columnWidth * (j + 1), rowRect.y, columnWidth, rowRect.height); // Add one blank column for the row labels.

						bool value = Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]];
						Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
						Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]] = value;

						// Draw checkbox tooltips.
						if (Mouse.IsOver(checkboxRect)) {
							TooltipHandler.TipRegion(checkboxRect, $"{$"BR.{pawnTypes[j]}".Translate().CapitalizeFirst()}: {$"BR.{informationCategories[i]}".Translate()}");
						}
					}
				}

				// Draw "learning enabled" label.
				Widgets.Label(learningEnabledLabelRect, "BR.LearningEnabled".Translate().CapitalizeFirst());
				if (Mouse.IsOver(learningEnabledLabelRect)) {
					TooltipHandler.TipRegion(learningEnabledLabelRect, MiscellaneousUtility.EndWithPeriod("BR.LearningEnabled.Desc".Translate().CapitalizeFirst()));
				}

				// Draw "learning enabled" row.
				for (int i = 0; i < pawnTypes.Length; i++) {
					Rect checkboxRect = new Rect(learningEnabledRect.x + columnWidth * (i + 1), learningEnabledRect.y, columnWidth, learningEnabledRect.height); // Add one blank column for the row labels.

					bool value = Settings.LearningEnabled[(int)pawnTypes[i]];
					Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
					Settings.LearningEnabled[(int)pawnTypes[i]] = value;
				}
			}

			Rect listingRect = new Rect(scrollViewRect.x, learningEnabledRect.yMax, scrollViewRect.width, scrollViewRect.height - tableRect.height - learningEnabledLabelRect.height - learningEnabledRect.height);
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(listingRect);

			if (KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Traits, true)) {
#if V1_0 || V1_1 || V1_2 || V1_3
				listing.Label($"{"BR.TraitDiscoveryDifficulty".Translate().CapitalizeFirst()} ({Settings.TraitsLearningDifficulty})");
				Settings.TraitsLearningDifficulty = (int)listing.Slider(Settings.TraitsLearningDifficulty, 0, 10);
#else
				Settings.TraitsLearningDifficulty = (int)listing.SliderLabeled($"{"BR.TraitDiscoveryDifficulty".Translate().CapitalizeFirst()} ({Settings.TraitsLearningDifficulty})", Settings.TraitsLearningDifficulty, 0, 10, tooltip: "BR.TraitDiscoveryDifficulty.Desc".Translate().CapitalizeFirst());
#endif
			}

			if (KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Backstory, true)) {
#if V1_0 || V1_1 || V1_2 || V1_3
				listing.Label($"{"BR.BackstoryDiscoveryDifficulty".Translate().CapitalizeFirst()} ({Settings.BackstoryLearningDifficulty})");
				Settings.BackstoryLearningDifficulty = (int)listing.Slider(Settings.BackstoryLearningDifficulty, 0, 10);
#else
				Settings.BackstoryLearningDifficulty = (int)listing.SliderLabeled($"{"BR.BackstoryDiscoveryDifficulty".Translate().CapitalizeFirst()} ({Settings.BackstoryLearningDifficulty})", Settings.BackstoryLearningDifficulty, 0, 10, tooltip: "BR.BackstoryDiscoveryDifficulty.Desc".Translate().CapitalizeFirst());
#endif
			}

			if (KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Skills, true)) {
#if V1_0 || V1_1 || V1_2 || V1_3
				listing.Label($"{"BR.SkillsDiscoveryDifficulty".Translate().CapitalizeFirst()} ({Settings.SkillsLearningDifficulty})");
				Settings.SkillsLearningDifficulty = (int)listing.Slider(Settings.SkillsLearningDifficulty, 0, 10);
#else
				Settings.SkillsLearningDifficulty = (int)listing.SliderLabeled($"{"BR.SkillsDiscoveryDifficulty".Translate().CapitalizeFirst()} ({Settings.SkillsLearningDifficulty})", Settings.SkillsLearningDifficulty, 0, 10, tooltip: "BR.SkillsDiscoveryDifficulty.Desc".Translate().CapitalizeFirst());
#endif
			}

			if (!Settings.LegacyMode && KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Traits)) {
				bool enableUniqueTraitUnlockConditions = Settings.EnableUniqueTraitUnlockConditions;
				listing.CheckboxLabeled("BR.EnableUniqueTraitUnlockConditions".Translate().CapitalizeFirst(), ref enableUniqueTraitUnlockConditions, "BR.EnableUniqueTraitUnlockConditions.Desc".Translate().CapitalizeFirst());
				Settings.EnableUniqueTraitUnlockConditions = enableUniqueTraitUnlockConditions;
			}

			if (!KnowledgeUtility.IsAllInformationKnownFor(PawnType.Colonist) || KnowledgeUtility.IsAnyLearningEnabledFor(PawnType.Colonist)) {
				bool alwaysKnowStartingColonists = Settings.AlwaysKnowStartingColonists;
				listing.CheckboxLabeled("BR.AlwaysKnowStartingColonists".Translate().CapitalizeFirst(), ref alwaysKnowStartingColonists, "BR.AlwaysKnowStartingColonists.Desc".Translate().CapitalizeFirst());
				Settings.AlwaysKnowStartingColonists = alwaysKnowStartingColonists;
			}

			if (!KnowledgeUtility.IsInformationSupersetOfAny(PawnType.Colonist) || KnowledgeUtility.IsAnyLearningEnabledForAny()) {
				bool rememberFormerColonists = Settings.RememberFormerColonists;
				listing.CheckboxLabeled("BR.RememberFormerColonists".Translate().CapitalizeFirst(), ref rememberFormerColonists, "BR.RememberFormerColonists.Desc".Translate().CapitalizeFirst());
				Settings.RememberFormerColonists = rememberFormerColonists;
			}

			if (!KnowledgeUtility.IsInformationKnownForAll(InformationCategory.Basic) || !KnowledgeUtility.IsInformationKnownForAll(InformationCategory.Traits) || !KnowledgeUtility.IsInformationKnownForAll(InformationCategory.Backstory)) {
				bool alwaysKnowMoreAboutColonistRelatives = Settings.AlwaysKnowMoreAboutColonistRelatives;
				listing.CheckboxLabeled("BR.AlwaysKnowMoreAboutColonistRelatives".Translate().CapitalizeFirst(), ref alwaysKnowMoreAboutColonistRelatives, "BR.AlwaysKnowMoreAboutColonistRelatives.Desc".Translate().CapitalizeFirst());
				Settings.AlwaysKnowMoreAboutColonistRelatives = alwaysKnowMoreAboutColonistRelatives;
			}

			if (KnowledgeUtility.IsLearningEnabledFor(InformationCategory.Traits, PawnType.Colonist)) {
				bool alwaysKnowGrowthMoments = Settings.AlwaysKnowGrowthMomentTraits;
				listing.CheckboxLabeled("BR.AlwaysKnowGrowthMomentTraits".Translate().CapitalizeFirst(), ref alwaysKnowGrowthMoments, "BR.AlwaysKnowGrowthMomentTraits.Desc".Translate().CapitalizeFirst());
				Settings.AlwaysKnowGrowthMomentTraits = alwaysKnowGrowthMoments;
			}

			if (!KnowledgeUtility.IsAllInformationKnownFor(PawnType.Colonist) || !KnowledgeUtility.IsAllInformationKnownFor(PawnType.Controlled)) {
				bool neverHideControls = Settings.NeverHideControls;
				listing.CheckboxLabeled("BR.NeverHideControls".Translate().CapitalizeFirst(), ref neverHideControls, "BR.NeverHideControls.Desc".Translate().CapitalizeFirst());
				Settings.NeverHideControls = neverHideControls;
			}

			if (!KnowledgeUtility.IsAllInformationKnownForAll()) {
				bool neverHideMessages = Settings.NeverHideMessages;
				listing.CheckboxLabeled("BR.NeverHideMessages".Translate().CapitalizeFirst(), ref neverHideMessages, "BR.NeverHideMessages.Desc".Translate().CapitalizeFirst());
				Settings.NeverHideMessages = neverHideMessages;

				bool neverHideLetters = Settings.NeverHideLetters;
				listing.CheckboxLabeled("BR.NeverHideLetters".Translate().CapitalizeFirst(), ref neverHideLetters, "BR.NeverHideLetters.Desc".Translate().CapitalizeFirst());
				Settings.NeverHideLetters = neverHideLetters;

				bool neverHideTextMotes = Settings.NeverHideTextMotes;
				listing.CheckboxLabeled("BR.NeverHideTextMotes".Translate().CapitalizeFirst(), ref neverHideTextMotes, "BR.NeverHideTextMotes.Desc".Translate().CapitalizeFirst());
				Settings.NeverHideTextMotes = neverHideTextMotes;

				bool neverHideAlerts = Settings.NeverHideAlerts;
				listing.CheckboxLabeled("BR.NeverHideAlerts".Translate().CapitalizeFirst(), ref neverHideLetters, "BR.NeverHideAlerts.Desc".Translate().CapitalizeFirst());
				Settings.NeverHideAlerts = neverHideAlerts;

				bool hideFactionInformation = Settings.HideFactionInformation;
				listing.CheckboxLabeled("BR.HideFactionInformation".Translate().CapitalizeFirst(), ref hideFactionInformation, "BR.HideFactionInformation.Desc".Translate().CapitalizeFirst());
				Settings.HideFactionInformation = hideFactionInformation;
			}

			if (!Settings.LegacyMode) {
				bool hideAncientCorpses = Settings.HideAncientCorpses;
				listing.CheckboxLabeled("BR.HideAncientCorpses".Translate().CapitalizeFirst(), ref hideAncientCorpses, "BR.HideAncientCorpses.Desc".Translate().CapitalizeFirst());
				Settings.HideAncientCorpses = hideAncientCorpses;
			}

			bool legacyMode = Settings.LegacyMode;
			listing.CheckboxLabeled("BR.LegacyMode".Translate().CapitalizeFirst(), ref legacyMode, "BR.LegacyMode.Desc".Translate().CapitalizeFirst());
			Settings.LegacyMode = legacyMode;

			if (!Settings.AlwaysKnowStartingColonists && !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, PawnType.Colonist, ControlCategory.Control)) {
#if V1_0
				listing.Label(MiscellaneousUtility.EndWithPeriod("BR.WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst()), tipSignal: "BR.WarningDisabledBasicForStartingColonists.Desc".Translate().CapitalizeFirst());
#elif V1_1 || V1_2
				_ = listing.Label(MiscellaneousUtility.EndWithPeriod("BR.WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst()).Resolve().Colorize(ColoredText.WarningColor), tipSignal: "BR.WarningDisabledBasicForStartingColonists.Desc".Translate().CapitalizeFirst());
#else
				_ = listing.Label(MiscellaneousUtility.EndWithPeriod("BR.WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst()).Colorize(ColoredText.WarningColor), tipSignal: "BR.WarningDisabledBasicForStartingColonists.Desc".Translate().CapitalizeFirst());
#endif
			}

			listing.End();
			this.totalSettingsHeight = listingRect.y + listing.CurHeight;
			Widgets.EndScrollView();
		}

		public override string SettingsCategory() => "BR.BoundedRationality".Translate().CapitalizeFirst();
	}
}
