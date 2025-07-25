using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetMod : Mod {
		private const float CheckboxSize = 24;

		public static WellMetSettings Settings { get; private set; }

		public WellMetMod(ModContentPack content) : base(content) => Settings = this.GetSettings<WellMetSettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			base.DoSettingsWindowContents(inRect);

			PawnType[] pawnTypes = (PawnType[])Enum.GetValues(typeof(PawnType));
			InformationCategory[] informationCategories = (InformationCategory[])Enum.GetValues(typeof(InformationCategory));

			float columnWidth = inRect.width / (pawnTypes.Length + 1);
			float rowHeight = Math.Max(Text.LineHeight, CheckboxSize);

			// Draw column labels.
			for (int i = 0; i < pawnTypes.Length; i++) {
				Rect rect = new Rect(inRect.x + columnWidth * (i + 1), inRect.y, columnWidth, rowHeight);
				Widgets.Label(rect, pawnTypes[i].ToString().Translate().CapitalizeFirst().Resolve());

				// Draw label tooltips.
				if (Mouse.IsOver(rect)) {
					TooltipHandler.TipRegion(rect, (pawnTypes[i].ToString() + "Blurb").Translate().CapitalizeFirst().EndWithPeriod().Resolve());
				}
			}

			// Draw rows.
			for (int i = 0; i < informationCategories.Length; i++) {
				// Draw row label.
				Rect rect = new Rect(inRect.x, inRect.y + rowHeight * (i + 1), columnWidth, rowHeight);
				Widgets.Label(rect, informationCategories[i].ToString().Translate().CapitalizeFirst().Resolve());

				// Draw label tooltips.
				if (Mouse.IsOver(rect)) {
					TooltipHandler.TipRegion(rect, (informationCategories[i].ToString() + "Blurb").Translate().CapitalizeFirst().EndWithPeriod().Resolve());
				}

				// Draw checkboxes.
				for (int j = 0; j < pawnTypes.Length; j++) {
					Rect checkboxRect = new Rect(inRect.x + columnWidth * (j + 1), inRect.y + rowHeight * (i + 1), CheckboxSize, CheckboxSize);

					bool value = KnowledgeUtility.IsInformationKnownFor(informationCategories[i], pawnTypes[j]);
					Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
					Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]] = value;

					// Draw checkbox tooltips.
					if (Mouse.IsOver(checkboxRect)) {
						TooltipHandler.TipRegion(checkboxRect, pawnTypes[j].ToString().Translate().CapitalizeFirst().Resolve() + ": " + informationCategories[i].ToString().Translate().Resolve());
					}
				}
			}

			float tableHeight = rowHeight * (informationCategories.Length + 1);
			Rect footerRect = new Rect(inRect.x, inRect.y + tableHeight, inRect.width, inRect.height - tableHeight);
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(footerRect);

			bool hideFactionInformation = Settings.HideFactionInformation;
			listing.CheckboxLabeled("HideFactionInformation".Translate().CapitalizeFirst().Resolve(), ref hideFactionInformation);
			Settings.HideFactionInformation = hideFactionInformation;

			bool alwaysKnowStartingColonists = Settings.AlwaysKnowStartingColonists;
			listing.CheckboxLabeled("AlwaysKnowStartingColonists".Translate().CapitalizeFirst().Resolve(), ref alwaysKnowStartingColonists);
			Settings.AlwaysKnowStartingColonists = alwaysKnowStartingColonists;

			if (!KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, PawnType.Colonist) && !Settings.AlwaysKnowStartingColonists) {
				_ = listing.Label("WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst().EndWithPeriod().Colorize(ColoredText.WarningColor));
			}

			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist)) {
				Settings.ColonistTraitDiscoveryDifficulty = (int)listing.SliderLabeled("ColonistTraitDiscoveryDifficulty".Translate(Settings.ColonistTraitDiscoveryDifficulty).CapitalizeFirst().Resolve(), Settings.ColonistTraitDiscoveryDifficulty, 0, 10);

				if (Settings.ColonistTraitDiscoveryDifficulty > 0) {
					bool alwaysKnowGrowthMoments = Settings.AlwaysKnowGrowthMomentTraits;
					listing.CheckboxLabeled("AlwaysKnowGrowthMomentTraits".Translate().CapitalizeFirst().Resolve(), ref alwaysKnowGrowthMoments);
					Settings.AlwaysKnowGrowthMomentTraits = alwaysKnowGrowthMoments;
				}
			}

			listing.End();
		}

		public override string SettingsCategory() => "WellMet".Translate().CapitalizeFirst().Resolve();
	}
}
