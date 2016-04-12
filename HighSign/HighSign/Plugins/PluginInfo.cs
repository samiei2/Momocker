﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HighSign.Common.Plugins;

namespace HighSign.Plugins
{
	public class PluginInfo : IPluginInfo
	{
		#region Private Instance Variables

		private string _DisplayText = null;

		#endregion

		#region Constructors

		public PluginInfo(IPlugin Plugin, string Class, string Filename)
		{
			this.Plugin = Plugin;
			this.Class = Class;
			this.Filename = Filename;
		}

		#endregion

		#region IPluginInfo Instance Properties

		public string DisplayText
		{
			get { return String.IsNullOrEmpty(_DisplayText) ? this.ToString() : _DisplayText; }
			set { _DisplayText = value; }
		}

		public IPlugin Plugin { get; set; }
		public string Class { get; set; }
		public string Filename { get; set; }

		#endregion

		#region Base Method Overrides

		public override string ToString()
		{
			return String.Format("{0} - {1}", Plugin.Category, Plugin.Name);
		}

		#endregion
	}
}
