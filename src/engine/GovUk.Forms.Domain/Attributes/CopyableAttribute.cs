using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Domain.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public sealed class CopyableAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public sealed class SummaryAttribute : Attribute;