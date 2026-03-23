using System.Xml.Linq;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain;

public class XmlFileUploadModelTests
{
    [Fact]
    public void Base64XmlAsString_GetXml_ReturnsValidXmlDocument()
    {
        XmlFileUploadModel xmlFileUpload = new()
        {
            Contents = "PD94bWwgdmVyc2lvbj0iMS4wIiBzdGFuZGFsb25lPSJ5ZXMiPz4NCjxuczE6UlAxNEEgeG1sbnM6bnMxPSJodHRwOi8vd3d3Lmlucy5nc2ku" +
                       "Z292LnVrL0ZpbGVVcGxvYWQvUlAxNEFfQXBwbGljYXRpb24iPg0KICA8bnMxOkVtcGxveWVlPg0KICAgIDxuczE6SGVhZGVyPg0KICAgICAg" +
                       "PG5zMTpDYXNlUmVmZXJlbmNlPkNOMTAwMDAxMTI8L25zMTpDYXNlUmVmZXJlbmNlPg0KICAgIDwvbnMxOkhlYWRlcj4NCiAgICA8bnMxOkVt" +
                       "cGxveWVyTmFtZT5CQU5HTEEgRklTSCBCQVpBQVIgTElNSVRFRDwvbnMxOkVtcGxveWVyTmFtZT4NCiAgICA8bnMxOkVtcGxveWVlTmFtZT4N" +
                       "CiAgICAgIDxuczE6U3VybmFtZT5FZG1vbmRzb248L25zMTpTdXJuYW1lPg0KICAgICAgPG5zMTpGb3JlbmFtZXM+QWRyaWFuPC9uczE6Rm9y" +
                       "ZW5hbWVzPg0KICAgICAgPG5zMTpUaXRsZT5NcjwvbnMxOlRpdGxlPg0KICAgIDwvbnMxOkVtcGxveWVlTmFtZT4NCiAgICA8bnMxOk5JQ2xh" +
                       "c3M+QzwvbnMxOk5JQ2xhc3M+DQogICAgPG5zMTpOSU5PPkJQMDExNzUyQzwvbnMxOk5JTk8+DQogICAgPG5zMTpEYXRlT2ZCaXJ0aD4xOTYz" +
                       "LTA2LTEwPC9uczE6RGF0ZU9mQmlydGg+DQogICAgPG5zMTpTdGFydERhdGU+MjAxNy0wMS0wMzwvbnMxOlN0YXJ0RGF0ZT4NCiAgICA8bnMx" +
                       "OkRhdGVOb3RpY2VHaXZlbj4yMDIwLTA5LTA5PC9uczE6RGF0ZU5vdGljZUdpdmVuPg0KICAgIDxuczE6RW5kRGF0ZT4yMDIwLTA5LTA5PC9u" +
                       "czE6RW5kRGF0ZT4NCiAgICA8bnMxOlBheURldGFpbHM+DQogICAgICA8bnMxOkJhc2ljUGF5UGVyV2Vlaz41MDA8L25zMTpCYXNpY1BheVBl" +
                       "cldlZWs+DQogICAgICA8bnMxOldlZWtseVBheURheT5TYXR1cmRheTwvbnMxOldlZWtseVBheURheT4NCiAgICAgIDxuczE6QXJyZWFyc09m" +
                       "UGF5Pg0KICAgICAgICA8bnMxOkFycmVhcnNPZlBheVBlcmlvZDE+DQogICAgICAgICAgPG5zMTpBT1AxU3RhcnREYXRlPjIwMjAtMDgtMDE8" +
                       "L25zMTpBT1AxU3RhcnREYXRlPg0KICAgICAgICAgIDxuczE6QU9QMUVuZERhdGU+MjAyMC0wOC0zMTwvbnMxOkFPUDFFbmREYXRlPg0KICAg" +
                       "ICAgICAgIDxuczE6QU9QT3dlZDE+MjEwMDwvbnMxOkFPUE93ZWQxPg0KICAgICAgICAgIDxuczE6QU9QUGF5VHlwZTE+Ym91bmNlZGNoZXF1" +
                       "ZTwvbnMxOkFPUFBheVR5cGUxPg0KICAgICAgICA8L25zMTpBcnJlYXJzT2ZQYXlQZXJpb2QxPg0KICAgICAgICA8bnMxOkFycmVhcnNPZlBh" +
                       "eVBlcmlvZDIgLz4NCiAgICAgICAgPG5zMTpBcnJlYXJzT2ZQYXlQZXJpb2QzIC8+DQogICAgICAgIDxuczE6QXJyZWFyc09mUGF5UGVyaW9k" +
                       "NCAvPg0KICAgICAgPC9uczE6QXJyZWFyc09mUGF5Pg0KICAgIDwvbnMxOlBheURldGFpbHM+DQogICAgPG5zMTpIb2xpZGF5Pg0KICAgICAg" +
                       "PG5zMTpIb2xpZGF5Tm90UGFpZD4NCiAgICAgICAgPG5zMTpIb2xpZGF5MSAvPg0KICAgICAgICA8bnMxOkhvbGlkYXkyIC8+DQogICAgICAg" +
                       "IDxuczE6SG9saWRheTMgLz4NCiAgICAgIDwvbnMxOkhvbGlkYXlOb3RQYWlkPg0KICAgIDwvbnMxOkhvbGlkYXk+DQogIDwvbnMxOkVtcGxv" +
                       "eWVlPg0KPC9uczE6UlAxNEE+"
        };

        XDocument document = xmlFileUpload.GetXml();

        Assert.NotNull(document.Root);
    }
}