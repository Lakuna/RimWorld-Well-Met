﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

[assembly: CLSCompliant(false)]
namespace Lakuna.WellMet {
	public class WellMetMod : Mod {
		public static WellMetSettings Settings { get; private set; }

		public WellMetMod(ModContentPack content) : base(content) => Settings = this.GetSettings<WellMetSettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.Label("DifficultyFactor".Translate().CapitalizeFirst() + " (" + Settings.DifficultyFactor + ")");
			Settings.DifficultyFactor = listing.Slider(Settings.DifficultyFactor, 0, 60);

			if (Prefs.DevMode) {
				bool allTraitsDiscovered = Settings.AllTraitsDiscovered;
				listing.CheckboxLabeled("AllTraitsDiscovered".Translate().CapitalizeFirst(), ref allTraitsDiscovered);
				Settings.AllTraitsDiscovered = allTraitsDiscovered;
			}

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() => "WellMet".Translate().CapitalizeFirst();
	}
}
