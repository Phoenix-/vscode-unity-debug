﻿using System;
using MonoDevelop.Debugger.Soft.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityDebug
{
	public static class UnityAttach
	{
		static readonly Dictionary<string, string> targetNameToProcessName = new Dictionary<string,string> 
		{
			{ "unity editor", "Unity Editor" },
			{ "osx player", "OSXPlayer" },
			{ "windows player", "WindowsPlayer" },
			{ "linux player", "LinuxPlayer" },
			{ "ios player", "iPhonePlayer" },
			{ "android player", "AndroidPlayer" }
		};


		public static IEnumerable<UnityProcessInfo> GetAttachableProcesses(string targetName)
		{
			var match = Regex.Match(targetName, "\\(([0-9]+)\\)");
			var processId = -1;
			if (match.Success)
			{
				processId = Convert.ToInt32(match.Groups[1].Value);
				targetName = targetName.Substring(0, targetName.IndexOf("(") - 1);
			}
			string processName;

			UnityProcessDiscovery.GetProcessOptions options = UnityProcessDiscovery.GetProcessOptions.All;

			if (!targetNameToProcessName.TryGetValue (targetName.ToLower (), out processName)) {
				processName = targetName;
			} else {
				if (processName == "Unity Editor")
					options = UnityProcessDiscovery.GetProcessOptions.Editor;
				else
					options = UnityProcessDiscovery.GetProcessOptions.Players;
			}

			var processes = UnityProcessDiscovery.GetAttachableProcesses (options).Result;

			foreach(var p in processes)
			{
				Log.Write ("Found Unity process: " + p.Name + " (" + p.Id + ")");
			}

			return processId == -1
				? processes.Where (p => p.Name.Contains(processName))
				: processes.Where(p => p.Name.Contains(processName) && p.Id == processId);
		}
	}
}

