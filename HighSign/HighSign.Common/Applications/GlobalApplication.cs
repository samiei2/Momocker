﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HighSign.Common.Applications
{
	public class GlobalApplication : ApplicationBase
	{
		#region IApplication Properties

		public override string Name
		{
			get { return "All Applicatons"; }
			set { /* Set only exists for deserialization purposes */ }
		}

		public override MatchUsing MatchUsing
		{
			get { return MatchUsing.All; }
			set { /* Set only exists for deserialization purposes */ }
		}

		#endregion
	}
}
