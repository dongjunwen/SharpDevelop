﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A list of key-value pairs within a solution file.
	/// </summary>
	public class SolutionSection : IReadOnlyDictionary<string, string>
	{
		public event EventHandler Changed = delegate { };
		
		static readonly char[] forbiddenChars = { '\n', '\r', '\0', '=' };
		
		static void Validate(string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (value == null)
				throw new ArgumentNullException("value");
			if (key.IndexOfAny(forbiddenChars) >= 0)
				throw new ArgumentException("key contains invalid characters", "key");
			if (value.IndexOfAny(forbiddenChars) >= 0)
				throw new ArgumentException("value contains invalid characters", "value");
		}
		
		string sectionName;
		string sectionType;
		List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
		
		public SolutionSection(string sectionName, string sectionType)
		{
			Validate(sectionName, sectionType);
			this.sectionName = sectionName;
			this.sectionType = sectionType;
		}
		
		/// <summary>
		/// Gets/Sets the section name
		/// </summary>
		public string SectionName {
			get {
				return sectionName;
			}
			set {
				if (sectionName != value) {
					Validate(value, sectionType);
					sectionName = value;
					Changed(this, EventArgs.Empty);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets the section type (e.g. 'preProject'/'postProject'/'preSolution'/'postSolution')
		/// </summary>
		public string SectionType {
			get {
				return sectionType;
			}
			set {
				if (sectionType != value) {
					Validate(sectionName, value);
					sectionType = value;
					Changed(this, EventArgs.Empty);
				}
			}
		}
		
		public int Count {
			get { return entries.Count; }
		}
		
		public void Add(string key, string value)
		{
			Validate(key, value);
			entries.Add(new KeyValuePair<string, string>(key, value));
			Changed(this, EventArgs.Empty);
		}

		public bool Remove(string key)
		{
			if (entries.RemoveAll(e => e.Key == key) > 0) {
				Changed(this, EventArgs.Empty);
				return true;
			} else {
				return false;
			}
		}

		public void Clear()
		{
			entries.Clear();
			Changed(this, EventArgs.Empty);
		}

		public bool ContainsKey(string key)
		{
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Key == key)
					return true;
			}
			return false;
		}

		public bool TryGetValue(string key, out string value)
		{
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Key == key) {
					value = entries[i].Value;
					return true;
				}
			}
			value = null;
			return false;
		}

		public string this[string key] {
			get {
				for (int i = 0; i < entries.Count; i++) {
					if (entries[i].Key == key) {
						return entries[i].Value;
					}
				}
				return null;
			}
			set {
				Validate(key, value);
				for (int i = 0; i < entries.Count; i++) {
					if (entries[i].Key == key) {
						entries[i] = new KeyValuePair<string, string>(key, value);
						Changed(this, EventArgs.Empty);
						return;
					}
				}
				Add(key, value);
			}
		}

		public IEnumerable<string> Keys {
			get {
				return entries.Select(e => e.Key);
			}
		}

		public IEnumerable<string> Values {
			get {
				return entries.Select(e => e.Value);
			}
		}
		
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return entries.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return entries.GetEnumerator();
		}
	}
}
