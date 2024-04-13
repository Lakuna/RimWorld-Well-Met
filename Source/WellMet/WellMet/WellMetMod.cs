using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetMod : Mod {
		public static WellMetSettings Settings { get; private set; }

		public WellMetMod(ModContentPack content) : base(content) => Settings = this.GetSettings<WellMetSettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			base.DoSettingsWindowContents(inRect);

			PawnType[] pawnTypes = (PawnType[])Enum.GetValues(typeof(PawnType));
			InformationCategory[] informationCategories = (InformationCategory[])Enum.GetValues(typeof(InformationCategory));

			float columnWidth = inRect.width / (pawnTypes.Length + 1);
			float rowHeight = Text.LineHeight;

			// Draw column labels.
			for (int i = 0; i < pawnTypes.Length; i++) {
				Rect rect = new Rect(inRect.x + columnWidth * (i + 1), inRect.y, columnWidth, rowHeight);
				Widgets.Label(rect, pawnTypes[i].ToString().Translate().CapitalizeFirst());
			}

			// Draw rows.
			for (int i = 0; i < informationCategories.Length; i++) {
				// Draw row label.
				Rect rect = new Rect(inRect.x, inRect.y + rowHeight * (i + 1), columnWidth, rowHeight);
				Widgets.Label(rect, informationCategories[i].ToString().Translate().CapitalizeFirst());

				// Draw checkboxes.
				for (int j = 0; j < pawnTypes.Length; j++) {
					bool value = KnowledgeUtility.IsInformationKnownFor(informationCategories[i], pawnTypes[j]);
					Widgets.Checkbox(new Vector2(inRect.x + columnWidth * (j + 1), inRect.y + rowHeight * (i + 1)), ref value);
					Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]] = value;
				}
			}

			float tableHeight = rowHeight * (informationCategories.Length + 1);
			Rect footerRect = new Rect(inRect.x, inRect.y + tableHeight, inRect.width, inRect.height - tableHeight);
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(footerRect);

			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist)) {
				Settings.TraitDiscoverSpeedFactor = (int)listing.SliderLabeled("TraitDiscoverSpeedFactor".Translate(Settings.TraitDiscoverSpeedFactor).CapitalizeFirst(), Settings.TraitDiscoverSpeedFactor, 0, 10);

				if (Settings.TraitDiscoverSpeedFactor > 0) {
					bool alwaysKnowStartingColonists = Settings.AlwaysKnowStartingColonists;
					listing.CheckboxLabeled("AlwaysKnowStartingColonists".Translate().CapitalizeFirst(), ref alwaysKnowStartingColonists);
					Settings.AlwaysKnowStartingColonists = alwaysKnowStartingColonists;

					bool alwaysKnowGrowthMoments = Settings.AlwaysKnowGrowthMoments;
					listing.CheckboxLabeled("AlwaysKnowGrowthMoments".Translate().CapitalizeFirst(), ref alwaysKnowGrowthMoments);
					Settings.AlwaysKnowGrowthMoments = alwaysKnowGrowthMoments;
				}
			}

			listing.End();
		}

		public override string SettingsCategory() => "WellMet".Translate().CapitalizeFirst();
	}
}
