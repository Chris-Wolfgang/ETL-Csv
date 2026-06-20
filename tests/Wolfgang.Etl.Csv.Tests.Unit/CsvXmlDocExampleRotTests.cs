#if NET8_0_OR_GREATER

using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Detects rot in <c>&lt;example&gt;&lt;code&gt;</c> XML-doc blocks across
/// the Wolfgang.Etl.Csv public API.
///
/// Reads the generated XML documentation file at runtime, extracts every
/// <c>code</c> block inside an <c>example</c> element, and parses each one
/// with the Roslyn syntax analyzer. Any syntax error counts as rot — a
/// rename, signature change, or missing-await typo that survived the
/// review but invalidates the doc.
///
/// This is a syntax-only check, not a full compilation. It catches the
/// typical regression (renamed method, missing semicolon, broken brace),
/// not unresolved-symbol issues that would require building against the
/// full assembly closure.
///
/// Restricted to net8.0+ because Microsoft.CodeAnalysis.CSharp doesn't
/// load cleanly on older TFMs. The check fires on at least one modern
/// TFM per CI run, which is sufficient — XML docs ship as a single .xml
/// file from the multi-TFM build, not per-TFM.
/// </summary>
public class CsvXmlDocExampleRotTests
{
    [Fact]
    public void Every_XML_doc_example_block_parses_as_valid_CSharp()
    {
        var xmlPath = LocateXmlDocFile();
        Assert.True(File.Exists(xmlPath), $"Generated XML doc file not found at {xmlPath}. Check GenerateDocumentationFile in csproj.");

        var doc = XDocument.Load(xmlPath);
        var codeBlocks = doc
            .Descendants("example")
            .Descendants("code")
            .Select(c => new
            {
                Member = c.Ancestors("member").FirstOrDefault()?.Attribute("name")?.Value ?? "(unknown member)",
                Code = c.Value,
            })
            .ToList();

        // Sanity: we expect at least one example. If this assertion fails
        // before any rot can occur, the XML doc file structure changed and
        // the test needs an update.
        Assert.NotEmpty(codeBlocks);

        // Parse in Regular kind with C# 10+ top-level statements semantics.
        // The file is treated as an implicit Main: top-level statements come
        // first, type declarations after — works for both kinds of examples
        // typical in XML docs ("here's a snippet to drop in a method" and
        // "here's a record type to decorate").
        //
        // LanguageVersion.Latest so modern syntax (records, file-scoped
        // namespaces, init-only properties, etc.) doesn't trip the parser.
        var options = new CSharpParseOptions(languageVersion: LanguageVersion.Latest);

        // Prefix: usings + a single top-level statement so the file parses
        // as a top-level program. After this prefix, the example body can be
        // either more top-level statements OR a type declaration (the latter
        // must come after statements per top-level-program rules — the trailing
        // example always satisfies that ordering since the prefix has at least
        // one statement). Symbol-resolution errors (CS0246 etc.) don't fire
        // because ParseText is syntax-only.
        const string prefix = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

var __probe__ = 0;
";

        var failures = new System.Collections.Generic.List<string>();
        foreach (var block in codeBlocks)
        {
            var wrapped = prefix + block.Code;
            var tree = CSharpSyntaxTree.ParseText(wrapped, options);
            var errors = tree.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToList();

            if (errors.Count > 0)
            {
                var firstError = errors[0];
                failures.Add($"Member '{block.Member}': example failed to parse — {firstError.Id} {firstError.GetMessage()} at offset {firstError.Location.SourceSpan.Start}");
            }
        }

        Assert.True
        (
            failures.Count == 0,
            "XML doc <example> block(s) failed Roslyn syntax parse:\n  - " + string.Join("\n  - ", failures)
        );
    }



    private static string LocateXmlDocFile()
    {
        // GenerateDocumentationFile=True writes the .xml next to the .dll.
        var assemblyLocation = typeof(CsvExtractor<>).Assembly.Location;
        var directory = Path.GetDirectoryName(assemblyLocation)!;
        var baseName = Path.GetFileNameWithoutExtension(assemblyLocation);
        return Path.Combine(directory, baseName + ".xml");
    }
}

#endif
