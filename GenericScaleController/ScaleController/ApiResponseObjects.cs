using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleController
{
	internal class UpdateLocationScaleDataResponse
	{
		public bool ZeroScale { get; set; }
		public bool TareScale { get; set; }
		public bool ClearScale { get; set; }
		public bool ResetAxles { get; set; }
		public bool FinishAxles { get; set; }
		public string? PrintDocument { get; set; }
	}
}
