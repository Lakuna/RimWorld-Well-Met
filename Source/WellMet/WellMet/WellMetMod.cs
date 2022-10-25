using System;
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

			listing.Label("Difficulty factor (" + Settings.DifficultyFactor + ")");
			Settings.DifficultyFactor = listing.Slider(Settings.DifficultyFactor, 0, 60);

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}
	}
}
