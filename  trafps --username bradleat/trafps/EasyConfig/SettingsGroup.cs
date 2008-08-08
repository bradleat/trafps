#region Copyright Notice
/*
 * Copyright (c) 2007 Nick Gravelyn
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace EasyConfig
{
	/// <summary>
	/// A group of settings from a configuration file.
	/// </summary>
	public class SettingsGroup
	{
		#region Fields and Properties

		string _name;
		Dictionary<string, Setting> _settings;

		/// <summary>
		/// Gets the name of the group.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the settings found in the group.
		/// </summary>
		public Dictionary<string, Setting> Settings
		{
			get { return _settings; }
		}

		#endregion

		#region Internal Constructors

		internal SettingsGroup(string name)
		{
			_name = name;
			_settings = new Dictionary<string, Setting>();
		}

		internal SettingsGroup(string name, List<Setting> settings)
		{
			_name = name;
			_settings = new Dictionary<string,Setting>();

			foreach (Setting setting in settings)
				_settings.Add(setting.Name, setting);
		}

		#endregion

		#region Adding Non-Array Settings

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, int value)
		{
			addSetting(name, value.ToString(), false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, float value)
		{
			addSetting(name, value.ToString(), false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, bool value)
		{
			addSetting(name, value.ToString(), false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, string value)
		{
			value = assertStringQuotes(value);
			addSetting(name, value, false);
		}

		#endregion

		#region Adding Array Settings

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params int[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params float[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params bool[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params string[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(assertStringQuotes(values[i]));
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		#endregion

		#region Deleting Settings

		/// <summary>
		/// Deletes a setting from the group.
		/// </summary>
		/// <param name="name">The name of the setting to delete.</param>
		public void DeleteSetting(string name)
		{
			_settings.Remove(name);
		}

		#endregion

		#region Private Helpers For Adding

		private static string assertStringQuotes(string value)
		{
			//make sure we have our surrounding quotations
			if (!value.StartsWith("\""))
				value = "\"" + value;
			if (!value.EndsWith("\""))
				value = value + "\"";
			return value;
		}

		private void addSetting(string name, string value, bool isArray)
		{
			_settings.Add(name, new Setting(name, value, isArray));
		}

		#endregion
	}
}
