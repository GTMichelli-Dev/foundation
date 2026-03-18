using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleController
{
	internal class ReceiveImageRequest
	{
		public int LocationId { get; set; }
		public string CameraName { get; set; } = string.Empty;
		public string Image { get; set; } = string.Empty;
		public bool OK { get; set; } = false;
		public string Message { get; set; } = string.Empty;
	}
}
