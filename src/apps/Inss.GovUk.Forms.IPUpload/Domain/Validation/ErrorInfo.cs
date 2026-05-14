using System;
using System.Collections.Generic;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorInfo : ErrorInfoHeader
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public int RowCount => Rows.Length > 0 ? Rows.Length - 1 : 0; // First row is the header
    
    public string[][] Rows { get; private set; } = [];

    public int ColumnCount => Rows[0].Length;

    public void AddRow(params string[] rowValues)
    {
        List<string[]> helper = [..Rows, rowValues];
        Rows = helper.ToArray();
    }
}