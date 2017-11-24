using System;
using System.IO;
using EnvDTE;

namespace ProductivityShell.Extensions
{
    internal static class ProjectExtensions
    {
        public static string GetOutputFolder(this Project project)
        {
            var directoryName = Path.GetDirectoryName(project.FullName);
            if (directoryName == null)
                throw new InvalidOperationException("Unable to find project directory");

            var property = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath");
            if (string.IsNullOrWhiteSpace(property.Value?.ToString()))
                throw new InvalidOperationException("OutputPath property is null or empty");

            return Path.Combine(directoryName, property.Value.ToString());
        }

        public static string GetOutputFilePath(this Project project)
        {
            var outputFolder = GetOutputFolder(project);
            var property = project.Properties.Item("OutputFileName");
            if (string.IsNullOrWhiteSpace(property.Value?.ToString()))
                throw new InvalidOperationException("OutputFileName property is null or empty");

            return Path.Combine(outputFolder, property.Value.ToString());
        }
    }
}