using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jfevia.ProductivityShell.Vsix.Settings
{
    internal static class SettingsFileGenerator
    {
        private static readonly Dictionary<AccessModifier, string> TokenByAccessModifier = new Dictionary<AccessModifier, string>
        {
            {AccessModifier.Public, "public"},
            {AccessModifier.Internal, "internal"}
        };

        private static readonly Dictionary<SettingScope, string> AttribtuteByScope = new Dictionary<SettingScope, string>
        {
            {SettingScope.User, "global::System.Configuration.ApplicationScopedSettingAttribute()"},
            {SettingScope.Application, "global::System.Configuration.UserScopedSettingAttribute()"}
        };

        /// <summary>
        ///     Writes the specified container to the file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="container">The container.</param>
        public static void Write(string filePath, SettingsContainer container)
        {
            var stringBuilder = new StringBuilder();

            AppendHeader(stringBuilder);
            AppendNamespace(stringBuilder, container);

            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        /// <summary>
        ///     Appends the namespace.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="container">The container.</param>
        private static void AppendNamespace(StringBuilder stringBuilder, SettingsContainer container)
        {
            stringBuilder.AppendLine($"namespace {container.Namespace} {{");
            AppendClass(stringBuilder, container);
            stringBuilder.AppendLine("}");
        }

        /// <summary>
        ///     Appends the class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="container">The container.</param>
        private static void AppendClass(StringBuilder stringBuilder, SettingsContainer container)
        {
            stringBuilder.AppendLine("    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]");
            stringBuilder.AppendLine("    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator\", \"15.1.0.0\")]");
            stringBuilder.AppendLine($"    {GetTokenAccessModifier(container.AccessModifier)} sealed partial class {container.Name} : global::System.Configuration.ApplicationSettingsBase {{");
            stringBuilder.AppendLine($"        private static {container.Name} defaultInstance = (({container.Name})(global::System.Configuration.ApplicationSettingsBase.Synchronized(new {container.Name}())));");
            stringBuilder.AppendLine($"        public static {container.Name} Default {{");
            stringBuilder.AppendLine("            get {");
            stringBuilder.AppendLine("                return defaultInstance;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");

            foreach (var setting in container.Settings)
            {
                stringBuilder.AppendLine($"        [{GetAttributeByScope(setting.Scope)}]");
                stringBuilder.AppendLine("        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]");
                stringBuilder.AppendLine($"        [global::System.Configuration.DefaultSettingValueAttribute(\"{setting.Value}\")]");
                stringBuilder.AppendLine($"        public {setting.Type.FullName} {setting.Name} {{");
                stringBuilder.AppendLine("            get {");
                stringBuilder.AppendLine($"                return (({setting.Type.FullName})(this[\"{setting.Name}\"]));");
                stringBuilder.AppendLine("            }");
                stringBuilder.AppendLine("        }");
            }
        }

        /// <summary>
        ///     Gets the token access modifier.
        /// </summary>
        /// <param name="accessModifier">The access modifier.</param>
        /// <returns>The token.</returns>
        private static string GetTokenAccessModifier(AccessModifier accessModifier)
        {
            return TokenByAccessModifier[accessModifier];
        }

        /// <summary>
        ///     Gets the attribute by scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>The attribute.</returns>
        private static string GetAttributeByScope(SettingScope scope)
        {
            return AttribtuteByScope[scope];
        }

        /// <summary>
        ///     Appends the header.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        private static void AppendHeader(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("//------------------------------------------------------------------------------");
            stringBuilder.AppendLine("// <auto-generated>");
            stringBuilder.AppendLine("//     This code was generated by a tool.");
            stringBuilder.AppendLine($"//     Runtime Version:{Environment.Version}");
            stringBuilder.AppendLine("//");
            stringBuilder.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            stringBuilder.AppendLine("//     the code is regenerated.");
            stringBuilder.AppendLine("// </auto-generated>");
            stringBuilder.AppendLine("//------------------------------------------------------------------------------");
        }
    }
}