using System;
using System.Collections.Generic;

namespace ProductivityShell.Commands.Project
{
    internal class ProjecItemEqualityComparer : IEqualityComparer<EnvDTE.ProjectItem>
    {
        public bool Equals(EnvDTE.ProjectItem x, EnvDTE.ProjectItem y)
        {
            return string.Equals(x.Properties.Item("FullPath")
                                  ?.ToString(), y.Properties.Item("FullPath")
                                                 ?.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(EnvDTE.ProjectItem projectItem)
        {
            return projectItem.Properties.Item("FullPath")
                              .GetHashCode();
        }
    }
}