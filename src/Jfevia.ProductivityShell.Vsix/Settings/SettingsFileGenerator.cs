using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jfevia.ProductivityShell.Vsix.Settings
{
    internal static class SettingsFileGenerator
    {
        private static readonly Dictionary<AccessModifier, TypeAttributes> TypeAttributeByAccessModifier = new Dictionary<AccessModifier, TypeAttributes>
        {
            {AccessModifier.Public, TypeAttributes.Public},
            {AccessModifier.Internal, TypeAttributes.NestedAssembly}
        };

        private static readonly CodeTypeReference SettingsBaseReference = new CodeTypeReference(typeof(ApplicationSettingsBase), CodeTypeReferenceOptions.GlobalReference);

        /// <summary>
        ///     Writes the specified container to the file path.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="container">The container.</param>
        public static void Write(SettingsFile file, SettingsContainer container)
        {
            var codeNamespace = new CodeNamespace(container.Namespace);
            AppendHeader(codeNamespace);
            AppendContent(codeNamespace, container);

            var provider = CodeDomProvider.CreateProvider(file.Language);
            using (var writer = new StreamWriter(File.OpenWrite(file.DesignerFilePath)))
            {
                provider.GenerateCodeFromNamespace(codeNamespace, writer, null);
            }
        }

        /// <summary>
        ///     Appends the content.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="container">The container.</param>
        private static void AppendContent(CodeNamespace codeNamespace, SettingsContainer container)
        {
            AppendClass(codeNamespace, container);
        }

        /// <summary>
        ///     Appends the class.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="container">The container.</param>
        private static void AppendClass(CodeNamespace codeNamespace, SettingsContainer container)
        {
            var codeTypeDeclaration = new CodeTypeDeclaration(container.Name);
            codeTypeDeclaration.TypeAttributes = GetTypeAttributes(container.AccessModifier);
            codeTypeDeclaration.IsClass = true;
            codeTypeDeclaration.IsPartial = true;
            codeTypeDeclaration.BaseTypes.Add(SettingsBaseReference);

            codeTypeDeclaration.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(CompilerGeneratedAttribute), CodeTypeReferenceOptions.GlobalReference)));

            var type = typeof(SettingsFileGenerator);
            codeTypeDeclaration.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(GeneratedCodeAttribute), CodeTypeReferenceOptions.GlobalReference),
                    new CodeAttributeArgument(string.Empty,
                        new CodePrimitiveExpression(type.FullName)),
                    new CodeAttributeArgument(string.Empty,
                        new CodePrimitiveExpression(type.Assembly.GetName().Version.ToString()))));

            AddSingletonInstance(codeTypeDeclaration, container);

            foreach (var setting in container.Settings)
                AppendMember(codeTypeDeclaration, setting);

            codeNamespace.Types.Add(codeTypeDeclaration);
        }

        /// <summary>
        ///     Appends the member.
        /// </summary>
        /// <param name="codeTypeDeclaration">The code type declaration.</param>
        /// <param name="setting">The setting.</param>
        private static void AppendMember(CodeTypeDeclaration codeTypeDeclaration, Setting setting)
        {
            var codeMemberProperty = new CodeMemberProperty();
            codeMemberProperty.Name = setting.Name;
            codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMemberProperty.Type = new CodeTypeReference(setting.Type, CodeTypeReferenceOptions.GlobalReference);

            // get { return ((setting.Type.FullName)(this["Setting.Name"])); }
            codeMemberProperty.HasGet = true;
            codeMemberProperty.GetStatements.AddRange(
                new CodeStatementCollection
                {
                    new CodeMethodReturnStatement(
                        new CodeCastExpression(codeMemberProperty.Type,
                            new CodeIndexerExpression(
                                new CodeThisReferenceExpression(),
                                new CodePrimitiveExpression(setting.Name))))
                });

            // set { this["Setting.Name"] = value; }
            if (setting.Scope != SettingScope.Application)
            {
                codeMemberProperty.HasSet = true;
                codeMemberProperty.SetStatements.AddRange(
                    new CodeStatementCollection
                    {
                        new CodeAssignStatement(
                            new CodeIndexerExpression(
                                new CodeThisReferenceExpression(),
                                new CodePrimitiveExpression(setting.Name)),
                            new CodePropertySetValueReferenceExpression())
                    }
                );
            }

            // Scope attribute
            var typeOfSettingScopeAttribute = GetAttributeTypeByScope(setting.Scope);
            codeMemberProperty.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeOfSettingScopeAttribute, CodeTypeReferenceOptions.GlobalReference)));

            // TODO: Add provider attributes
            // TODO: Add description attributes

            codeMemberProperty.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(DebuggerNonUserCodeAttribute), CodeTypeReferenceOptions.GlobalReference)));

            // TODO: Add connection string attributes
            // TODO: Add web reference attributes

            codeMemberProperty.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(DefaultSettingValueAttribute), CodeTypeReferenceOptions.GlobalReference),
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(GetSerializedValue(setting.Type, setting.DefaultValue)))));

            // TODO: Add roaming attributes
            codeTypeDeclaration.Members.Add(codeMemberProperty);
        }

        /// <summary>
        ///     Gets the serialized value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>The serialized value.</returns>
        private static string GetSerializedValue(Type type, object value)
        {
            var typeDescriptor = TypeDescriptor.GetConverter(type);
            return typeDescriptor.ConvertToString(value);
        }

        /// <summary>
        ///     Adds the singleton instance.
        /// </summary>
        /// <param name="codeTypeDeclaration">The code type declaration.</param>
        /// <param name="container">The container.</param>
        private static void AddSingletonInstance(CodeTypeDeclaration codeTypeDeclaration, SettingsContainer container)
        {
            var settingsReference = new CodeTypeReference(codeTypeDeclaration.Name);
            var singletonField = new CodeMemberField(settingsReference, "defaultInstance");
            var newInstanceExpression = new CodeObjectCreateExpression(container.Name);
            var synchronizedExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(SettingsBaseReference), "Synchronized", newInstanceExpression);

            singletonField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
            singletonField.InitExpression = new CodeCastExpression(codeTypeDeclaration.Name, synchronizedExpression);

            codeTypeDeclaration.Members.Add(singletonField);

            var codeMemberProperty = new CodeMemberProperty();
            codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            codeMemberProperty.Name = "Default";
            codeMemberProperty.Type = settingsReference;
            codeMemberProperty.HasGet = true;
            codeMemberProperty.HasSet = false;

            var codeFieldReferenceExpression = new CodeFieldReferenceExpression();
            codeFieldReferenceExpression.FieldName = singletonField.Name;

            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(codeFieldReferenceExpression));
            codeTypeDeclaration.Members.Add(codeMemberProperty);
        }

        /// <summary>
        ///     Gets the type attributes.
        /// </summary>
        /// <param name="accessModifier">The access modifier.</param>
        /// <returns>The type attributes.</returns>
        private static TypeAttributes GetTypeAttributes(AccessModifier accessModifier)
        {
            return TypeAttributes.Sealed | TypeAttributeByAccessModifier[accessModifier];
        }

        /// <summary>
        ///     Gets the attribute by scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>The attribute.</returns>
        private static Type GetAttributeTypeByScope(SettingScope scope)
        {
            return scope == SettingScope.User ? typeof(UserScopedSettingAttribute) : typeof(ApplicationScopedSettingAttribute);
        }

        /// <summary>
        ///     Appends the header.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        private static void AppendHeader(CodeNamespace codeNamespace)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------------------------------------------------------------------------");
            stringBuilder.AppendLine(" <auto-generated>");
            stringBuilder.AppendLine("     This code was generated by a tool.");
            stringBuilder.AppendLine($"     Runtime Version:{Environment.Version}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("     Changes to this file may cause incorrect behavior and will be lost if");
            stringBuilder.AppendLine("     the code is regenerated.");
            stringBuilder.AppendLine(" </auto-generated>");
            stringBuilder.Append("------------------------------------------------------------------------------");
            codeNamespace.Comments.Add(new CodeCommentStatement(stringBuilder.ToString()));
        }
    }
}