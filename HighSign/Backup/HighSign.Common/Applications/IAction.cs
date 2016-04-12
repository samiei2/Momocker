﻿using System;

namespace HighSign.Common.Applications
{
	public interface IAction
	{
		string ActionSettings { get; set; }
		string GestureName { get; set; }
		string Name { get; set; }
		string PluginClass { get; set; }
		string PluginFilename { get; set; }
	}
}
