﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {

	[UseTemplate]
	public partial class WinScreen : Panel {
		private WinPanel FirstPanel { get; set; }
		private WinPanel SecondPanel { get; set; }
		private WinPanel Thirdanel { get; set; }

	}
}
