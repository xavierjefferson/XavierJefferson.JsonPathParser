using System.Text;

namespace XavierJefferson.JsonPathParser.Interfaces;

/// <summary>
///     Parses JSON as specified by the used {@link com.jayway.jsonpath.spi.json.JsonProvider}.
/// </summary>
public interface IParseContext
{
    IDocumentContext Parse(string json);

    IDocumentContext Parse(object? json);

    IDocumentContext Parse(Stream stream);

    IDocumentContext Parse(Stream stream, Encoding charset);

    IDocumentContext Parse(FileInfo fileInfo);

    IDocumentContext ParseUtf8(byte[] json);

    [Obsolete]
    IDocumentContext Parse(Uri json);
}