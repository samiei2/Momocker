﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighSign.Common
{
	public interface ILoadable
	{
		#region Instance Methods

		// Shortcut method to instantiate an object
		void Load();

		#endregion
	}
}
