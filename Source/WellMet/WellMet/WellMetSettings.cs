using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetSettings : ModSettings {
		private float difficultyFactor;

		public float DifficultyFactor {
			get => this.difficultyFactor;
			set => this.difficultyFactor = value;
		}

		public WellMetSettings() => this.DifficultyFactor = 15;

		public override void ExposeData() {
			Scribe_Values.Look(ref this.difficultyFactor, nameof(this.difficultyFactor));
			base.ExposeData();
		}
	}
}
