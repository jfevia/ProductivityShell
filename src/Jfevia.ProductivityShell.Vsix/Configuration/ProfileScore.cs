using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.Vsix.Configuration
{
    internal class ProfileScore
    {
        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        public Profile Configuration { get; protected set; }

        /// <summary>
        ///     Gets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public double Score { get; protected set; }

        /// <summary>
        ///     Generates the score.
        /// </summary>
        /// <param name="profiles">The profiles.</param>
        /// <param name="currentProfile">The current profile.</param>
        /// <remarks>
        ///     The score is based on the projects and their respective properties in a profile. Some properties like the
        ///     display name do not affect the score given to a profile.
        /// </remarks>
        /// <returns>The score.</returns>
        public static IEnumerable<ProfileScore> Generate(IEnumerable<Profile> profiles, Profile currentProfile)
        {
            foreach (var profile in profiles)
            {
                double score = 0;
                foreach (var project in profile.Projects)
                foreach (var currentProject in currentProfile.Projects)
                {
                    score += GenerateScore(project.CommandLineArgs, currentProject.CommandLineArgs);
                    score += GenerateScore(project.IsRemoteDebuggingEnabled, currentProject.IsRemoteDebuggingEnabled);
                    score += GenerateScore(project.Name, currentProject.Name);
                    score += GenerateScore(project.RemoteDebuggingMachine, currentProject.RemoteDebuggingMachine);
                    score += GenerateScore(project.StartBrowserUrl, currentProject.StartBrowserUrl);
                    score += GenerateScore(project.StartExternalProgram, currentProject.StartExternalProgram);
                    score += GenerateScore(project.WorkingDirectory, currentProject.WorkingDirectory);
                }

                yield return new ProfileScore
                {
                    Configuration = profile,
                    Score = score
                };
            }
        }

        /// <summary>
        ///     Generates the score.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The score.</returns>
        private static double GenerateScore<T>(T source, T target)
        {
            if (source == null)
                return 0.0;

            if (!EqualityComparer<T>.Default.Equals(source, target))
                return -1.0;

            return 1.0;
        }
    }
}