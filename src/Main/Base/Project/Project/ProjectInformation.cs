﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Base class for <see cref="ProjectCreateInformation"/> and <see cref="ProjectLoadInformation"/>.
	/// </summary>
	public class ProjectInformation
	{
		public ProjectInformation(ISolution solution, FileName fileName)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.Solution = solution;
			this.FileName = fileName;
			this.ProjectName = fileName.GetFileNameWithoutExtension();
			this.ProjectSections = new List<SolutionSection>();
			this.ConfigurationMapping = new ConfigurationMapping();
			var solutionConfig = solution.ActiveConfiguration;
			// In unit tests, ActiveConfiguration mayb return null
			if (solutionConfig.Configuration != null && solutionConfig.Platform != null)
				this.ActiveProjectConfiguration = this.ConfigurationMapping.GetProjectConfiguration(solution.ActiveConfiguration);
			else
				this.ActiveProjectConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
		}
		
		public ISolution Solution { get; private set; }
		public FileName FileName { get; private set; }
		public ConfigurationMapping ConfigurationMapping { get; set; }
		public ConfigurationAndPlatform ActiveProjectConfiguration { get; set; }
		public IList<SolutionSection> ProjectSections { get; private set; }
		public string ProjectName { get; set; }
		
		public Guid IdGuid { get; set; }
		public Guid TypeGuid { get; set; }
		
	}
	
	/// <summary>
	/// Parameter object for loading an existing project.
	/// </summary>
	public class ProjectLoadInformation : ProjectInformation
	{
		internal bool? upgradeToolsVersion;
		
		IProgressMonitor progressMonitor = new DummyProgressMonitor();
		
		/// <summary>
		/// Gets/Sets the progress monitor used during the load.
		/// This property never returns null.
		/// </summary>
		public IProgressMonitor ProgressMonitor {
			get { return progressMonitor; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				progressMonitor = value;
			}
		}
		
		public ProjectLoadInformation(ISolution parentSolution, FileName fileName, string projectName)
			: base(parentSolution, fileName)
		{
			if (projectName == null)
				throw new ArgumentNullException("projectName");
			this.ProjectName = projectName;
		}
	}
	
	/// <summary>
	/// This class holds all information the language binding need to create
	/// a predefined project for their language, if no project template for a
	/// specific language is avaiable, the language binding shouldn't care about
	/// this stuff.
	/// </summary>
	public class ProjectCreateInformation : ProjectInformation
	{
		public ProjectCreateInformation(ISolution solution, FileName outputFileName)
			: base(solution, outputFileName)
		{
			this.IdGuid = Guid.NewGuid();
			this.RootNamespace = string.Empty;
		}
		
		public string RootNamespace { get; set; }
		public TargetFramework TargetFramework { get; set; }
		public bool InitializeTypeSystem { get; set; }
	}
}
