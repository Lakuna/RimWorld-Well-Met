using Verse;

namespace Lakuna.WellMet.Utility {
	// Based on `Verse.BoolGrid`.
	public class BooleanMatrix : IExposable {
		private int width;

		private int height;

		private bool[] values;

		public BooleanMatrix() { }

		public BooleanMatrix(int width, int height) {
			this.width = width;
			this.height = height;
			this.values = new bool[this.width * this.height];
		}

		public bool this[int x, int y] {
			get => this.values[x + y * this.width];
			set => this.values[x + y * this.width] = value;
		}

		public void ExposeData() {
			Scribe_Values.Look(ref this.width, nameof(this.width));
			Scribe_Values.Look(ref this.height, nameof(this.height));
			DataExposeUtility.LookBoolArray(ref this.values, this.width * this.height, nameof(this.values));
		}
	}
}
