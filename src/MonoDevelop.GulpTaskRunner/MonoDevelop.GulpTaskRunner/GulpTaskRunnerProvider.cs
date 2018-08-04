//
// GulpTaskRunnerProvider.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2018 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TaskRunnerExplorer;

namespace MonoDevelop.GulpTaskRunner
{
	[TaskRunnerExport ("gulpfile.js")]
	class GulpTaskRunnerProvider : ITaskRunner
	{
		public List<ITaskRunnerOption> Options { get; set; }

		public async Task<ITaskRunnerConfig> ParseConfig (ITaskRunnerCommandContext context, string configPath)
		{
			return await Task.Run (async () => {
				ITaskRunnerNode hierarchy = await LoadHierarchy (configPath);
				return new GulpTaskRunnerConfig (hierarchy);
			});
		}

		async Task<ITaskRunnerNode> LoadHierarchy (string configPath)
		{
			string workingDirectory = Path.GetDirectoryName (configPath);

			var root = new TaskRunnerNode ("Gulp Task Runner");

			foreach (string task in await GulpCommandRunner.FindGulpTasks (workingDirectory)) {
				root.Children.Add (new TaskRunnerNode (task, true) {
					Description = string.Format ("Runs Gulp task {0}", task),
					Command = new TaskRunnerCommand (workingDirectory, "gulp", task)
				});
			}

			return root;
		}
	}
}
