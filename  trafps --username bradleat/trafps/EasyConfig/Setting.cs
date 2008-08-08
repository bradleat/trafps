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
using System.Text.RegularExpressions;

namespace EasyConfig
{
	/// <summary>
	/// A single setting from a configuration file
	/// </summary>
	public class Setting
	{
		#region Fields and Properties

		string _name;
		string _rawValue;
		bool _isArray = false;

		/// <summary>
		/// Gets the name of the setting.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the raw value of the setting.
		/// </summary>
		public string RawValue
		{
			get { return _rawValue; }
		}

		/// <summary>
		/// Gets whether or not the setting is an array.
		/// </summary>
		public bool IsArray
		{
			get { return _isArray; }
		}

		#endregion

		#region Internal Constructors

		internal Setting(string name, string value, bool isArray)
		{
			_name = name;
			_rawValue = value;
			_isArray = isArray;
		}

		#endregion

		#region Getting Value

		/// <summary>
		/// Attempts to return the setting's value as an integer.
		/// </summary>
		/// <returns>An integer representation of the value</returns>
		public int GetValueAsInt()
		{
			return int.Parse(RawValue);
		}

		/// <summary>
		/// Attempts to return the setting's value as a float.
		/// </summary>
		/// <returns>A float representation of the value</returns>
		public float GetValueAsFloat()
		{
			return float.Parse(RawValue);
		}

		/// <summary>
		/// Attempts to return the setting's value as a bool.
		/// </summary>
		/// <returns>A bool representation of the value</returns>
		public bool GetValueAsBool()
		{
			return bool.Parse(RawValue);
		}

		/// <summary>
		/// Attempts to return the setting's value as a string.
		/// </summary>
		/// <returns>A string representation of the value</returns>
		public string GetValueAsString()
		{
			if (!RawValue.StartsWith("\"") || !RawValue.EndsWith("\""))
				throw new Exception("Cannot convert value to string.");

			return RawValue.Substring(1, RawValue.Length - 2);
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of integers.
		/// </summary>
		/// <returns>An integer array representation of the value</returns>
		public int[] GetValueAsIntArray()
		{
			string[] parts = RawValue.Split(',');

			int[] valueParts = new int[parts.Length];

			for (int i = 0; i < parts.Length; i++)
				valueParts[i] = int.Parse(parts[i]);

			return valueParts;
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of floats.
		/// </summary>
		/// <returns>An float array representation of the value</returns>
		public float[] GetValueAsFloatArray()
		{
			string[] parts = RawValue.Split(',');

			float[] valueParts = new float[parts.Length];

			for (int i = 0; i < parts.Length; i++)
				valueParts[i] = float.Parse(parts[i]);

			return valueParts;
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of bools.
		/// </summary>
		/// <returns>An bool array representation of the value</returns>
		public bool[] GetValueAsBoolArray()
		{
			string[] parts = RawValue.Split(',');

			bool[] valueParts = new bool[parts.Length];

			for (int i = 0; i < parts.Length; i++)
				valueParts[i] = bool.Parse(parts[i]);

			return valueParts;
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of strings.
		/// </summary>
		/// <returns>An string array representation of the value</returns>
		public string[] GetValueAsStringArray()
		{
			Match match = Regex.Match(RawValue, "[\\\"][a-zA-Z\\d\\s]*[\\\"][,]*");

			List<string> values = new List<string>();

			while (match.Success)
			{
				string value = match.Value;
				if (value.EndsWith(","))
					value = value.Substring(0, value.Length - 1);

				value = value.Substring(1, value.Length - 2);
				values.Add(value);
				match = match.NextMatch();
			}

			return values.ToArray();
		}

		#endregion

		#region Setting Value

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(int value)
		{
			_rawValue = value.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(float value)
		{
			_rawValue = value.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(bool value)
		{
			_rawValue = value.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(string value)
		{
			_rawValue = assertStringQuotes(value);
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params int[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			_rawValue = builder.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params float[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			_rawValue = builder.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params bool[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			_rawValue = builder.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params string[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(assertStringQuotes(values[i]));
				if (i < values.Length - 1)
					builder.Append(",");
			}

			_rawValue = builder.ToString();
		}

		#endregion

		#region Private Helpers For Setting Value

		private static string assertStringQuotes(string value)
		{
			//make sure we have our surrounding quotations
			if (!value.StartsWith("\""))
				value = "\"" + value;
			if (!value.EndsWith("\""))
				value = value + "\"";
			return value;
		}

		#endregion
	}
}
