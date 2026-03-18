namespace ScaleController
{
	public class ScaleData
	{
		public int LocationID { get; set; }
		public int ScaleID { get; set; }

		public string Status { get; set; } = string.Empty;

		public int GrossWt { get; set; }
		public int? Axle1Wt { get; set; }
		public int? Axle2Wt { get; set; }
		public int? Axle3Wt { get; set; }
		public int? Axle4Wt { get; set; }

		public bool RedIndicatorStatus { get; set; }
		public bool GrnIndicatorStatus { get; set; }
	}
}
