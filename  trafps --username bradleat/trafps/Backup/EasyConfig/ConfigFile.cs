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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EasyConfig
{
	/// <summary>
	/// Represents a configuration file.
	/// </summary>
	public class ConfigFile
	{
		#region Fields and Properties

		Dictionary<string, SettingsGroup> _groups;

		/// <summary>
		/// Gets the groups found in the configuration file.
		/// </summary>
		public Dictionary<string, SettingsGroup> SettingGroups
		{
			get { return _groups; }
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Creates a blank configuration file.
		/// </summary>
		public ConfigFile()
		{
			_groups = new Dictionary<string, SettingsGroup>();
		}

		/// <summary>
		/// Loads a configuration file.
		/// </summary>
		/// <param name="file">The filename where the configuration file can be found.</param>
		public ConfigFile(string file)
		{
			LoadFromFile(file);
		}

		#endregion

		#region Adding/Deleting Groups

		/// <summary>
		/// Adds a new settings group to the configuration file.
		/// </summary>
		/// <param name="groupName">The name of the group.</param>
		/// <returns>The newly created SettingsGroup.</returns>
		public SettingsGroup AddSettingsGroup(string groupName)
		{
			if (_groups.ContainsKey(groupName))
				throw new Exception("Group already exists with name '" + groupName + "'");

			SettingsGroup group = new SettingsGroup(groupName);
			_groups.Add(groupName, group);

			return group;
		}

		/// <summary>
		/// Deletes a settings group from the configuration file.
		/// </summary>
		/// <param name="groupName">The name of the group to delete.</param>
		public void DeleteSettingsGroup(string groupName)
		{
			_groups.Remove(groupName);
		}

		#endregion

		#region Loading/Saving

		/// <summary>
		/// Loads the configuration from a file.
		/// </summary>
		/// <param name="file">The file from which to load the configuration.</param>
		public void LoadFromFile(string file)
		{
			//track line numbers for exceptions
			int lineNumber = 0;

			//groups found
			List<SettingsGroup> groups = new List<SettingsGroup>();

			//current group information
			string currentGroupName = null;
			List<Setting> settings = null;

			using (StreamReader reader = new StreamReader(file))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					lineNumber++;

					//strip out comments
					if (line.Contains("#"))
					{
						if (line.IndexOf("#") == 0)
							continue;

						line = line.Substring(0, line.IndexOf("#"));
					}

					//trim off any extra whitespace
					line = line.Trim();

					//try to match a group name
					Match match = Regex.Match(line, "\\[[a-zA-Z\\d\\s]+\\]");

					//found group name
					if (match.Success)
					{
						//if we have a current group we're on, we save it
						if (settings != null && currentGroupName != null)
							groups.Add(new SettingsGroup(currentGroupName, settings));

						//make sure the name exists
						if (match.Value.Length == 2)
							throw new Exception(string.Format("Group must have name (line {0})", lineNumber));

						//set our current group information
						currentGroupName = match.Value.Substring(1, match.Length - 2);
						settings = new List<Setting>();
					}

					//no group name, check for setting with equals sign
					else if (line.Contains("="))
					{
						//split the line
						string[] parts = line.Split('=');

						//if we have any more than 2 parts, we have a problem
						if (parts.Length != 2)
							throw new Exception(string.Format("Settings must be in the format 'name = value' (line {0})", lineNumber));

						//trim off whitespace
						parts[0] = parts[0].Trim();
						parts[1] = parts[1].Trim();

						//figure out if we have an array or not
						bool isArray = false;
						bool inString = false;

						//go through the characters
						foreach (char c in parts[1])
						{
							//any comma not in a string makes us creating an array
							if (c == ',' && !inString)
								isArray = true;

							//flip the inString value each time we hit a quote
							else if (c == '"')
								inString = !inString;
						}

						//if we have an array, we have to trim off whitespace for each item and
						//do some checking for boolean values.
						if (isArray)
						{
							//split our value array
							string[] pieces = parts[1].Split(',');

							//need to build a new string
							StringBuilder builder = new StringBuilder();

							for (int i = 0; i < pieces.Length; i++)
							{
								//trim off whitespace
								string s = pieces[i].Trim();

								//convert to lower case
								string t = s.ToLower();

								//check for any of the true values
								if (t == "on" || t == "yes" || t == "true")
									s = "true";

								//check for any of the false values
								else if (t == "off" || t == "no" || t == "false")
									s = "false";

								//append the value
								builder.Append(s);

								//if we are not on the last value, add a comma
								if (i < pieces.Length - 1)
									builder.Append(",");
							}

							//save the built string as the value
							parts[1] = builder.ToString();
						}

						//if not an array
						else
						{
							//make sure we are not working with a string value
							if (!parts[1].StartsWith("\""))
							{
								//convert to lower
								string t = parts[1].ToLower();

								//check for any of the true values
								if (t == "on" || t == "yes" || t == "true")
									parts[1] = "true";

								//check for any of the false values
								else if (t == "off" || t == "no" || t == "false")
									parts[1] = "false";
							}
						}

						//add the setting to our list making sure, once again, we have stripped
						//off the whitespace
						settings.Add(new Setting(parts[0].Trim(), parts[1].Trim(), isArray));
					}
				}
			}

			//make sure we save off the last group
			if (settings != null && currentGroupName != null)
				groups.Add(new SettingsGroup(currentGroupName, settings));

			//create our new group dictionary
			_groups = new Dictionary<string, SettingsGroup>();

			//add each group to the dictionary
			foreach (SettingsGroup group in groups)
				_groups.Add(group.Name, group);
		}

		/// <summary>
		/// Saves the configuration to a file
		/// </summary>
		/// <param name="filename">The filename for the saved configuration file.</param>
		public void Save(string filename)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				foreach (KeyValuePair<string, SettingsGroup> groupValue in _groups)
				{
					writer.WriteLine("[{0}]", groupValue.Key);

					foreach (KeyValuePair<string, Setting> settingValue in groupValue.Value.Settings)
						writer.WriteLine("{0} = {1}", settingValue.Key, settingValue.Value.RawValue);

					writer.WriteLine();
				}
			}
		}

		#endregion
	}
}
