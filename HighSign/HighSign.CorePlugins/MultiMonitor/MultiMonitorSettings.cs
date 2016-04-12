﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace HighSign.CorePlugins.Volume
{
	[DataContract]
	public class MultiMonitorSettings
	{
		#region Public Properties

		[DataMember]
		//Tri-state, matches cboMethod.Items indexes 0 = Increase, 1 = Decrease, 2 = Toggle Mute
		public int Method { get; set; }

		[DataMember]
		//Stored as actual percent, whole number 10, 20, etc.
		public int Percent { get; set; }


		#endregion
	}
}
