using Microsoft.Azure.Cosmos;
using System.Net;

namespace INSS.Platform.Cache.Infrastructure.Tests;

public class FeedResponseStub<T> : FeedResponse<T>
{
    private readonly IEnumerable<T> _items;

    public FeedResponseStub(IEnumerable<T> items)
    {
        _items = items;
    }

    public override string ContinuationToken => string.Empty;

    public override int Count => _items.Count();

    public override Headers Headers => new ();

    public override IEnumerable<T> Resource => _items;

    public override string IndexMetrics => throw new NotImplementedException();

    public override HttpStatusCode StatusCode => throw new NotImplementedException();

    public override CosmosDiagnostics Diagnostics => throw new NotImplementedException();

    public override IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
}
