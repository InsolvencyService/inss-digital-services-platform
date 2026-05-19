namespace Inss.Common;

public readonly record struct Error(string Description, ErrorType Type);