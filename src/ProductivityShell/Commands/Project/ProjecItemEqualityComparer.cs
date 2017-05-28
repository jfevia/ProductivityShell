using System;
using System.Collections.Generic;
using EnvDTE;

namespace ProductivityShell.Commands.Project
{
    internal class ProjecItemEqualityComparer : IEqualityComparer<ProjectItem>
    {
        public bool Equals(ProjectItem x, ProjectItem y)
        {
            return string.Equals(x.Properties.Item("FullPath")
                                  ?.ToString(), y.Properties.Item("FullPath")
                                                 ?.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ProjectItem projectItem)
        {
            return projectItem.Properties.Item("FullPath")
                              .GetHashCode();
        }
    }
}