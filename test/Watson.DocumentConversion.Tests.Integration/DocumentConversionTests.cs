using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Watson.DocumentConversion.Enums;
using Watson.DocumentConversion.Models;
using Xunit;

// ReSharper disable ExceptionNotDocumented

namespace Watson.DocumentConversion.Tests.Integration
{
    public class DocumentConversionTests
    {
        public DocumentConversionSettings Settings => GetSettings();

        public static IEnumerable<object[]> Files => new[]
        {
            new object[] {"SampleContent/html.html", FileType.Html},
            new object[] {"SampleContent/doc.doc", FileType.MsWordDoc},
            new object[] {"SampleContent/docx.docx", FileType.MsWordDocx},
            new object[] {"SampleContent/pdf.pdf", FileType.Pdf},
            new object[] {"SampleContent/xml.xml", FileType.Xml}
        };

        private DocumentConversionSettings GetSettings()
        {
            var content = File.ReadAllText("config.json");
            var settings = JsonConvert.DeserializeObject<DocumentConversionSettings>(content);
            return settings;
        }

        [Theory, MemberData("Files")]
        public async Task ConvertDocumentToAnswersAsync(string fileName, FileType fileType)
        {
            var service = new DocumentConversionService(Settings.Username, Settings.Password);
            IAnswers answers;

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                answers = await service.ConvertDocumentToAnswersAsync(fs, fileType);
            }

            Assert.NotNull(answers);
            Assert.True(answers.AnswerUnits.Any());
        }

        [Theory, MemberData("Files")]
        public async Task ConvertDocumentToHtmlAsync(string fileName, FileType fileType)
        {
            var service = new DocumentConversionService(Settings.Username, Settings.Password);
            string html;

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                html = await service.ConvertDocumentToHtmlAsync(fs, fileType);
            }

            Assert.False(string.IsNullOrWhiteSpace(html));
        }

        [Theory, MemberData("Files")]
        public async Task ConvertDocumentToTextAsync(string fileName, FileType fileType)
        {
            var service = new DocumentConversionService(Settings.Username, Settings.Password);
            string text;

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                text = await service.ConvertDocumentToTextAsync(fs, fileType);
            }

            Assert.False(string.IsNullOrWhiteSpace(text));
        }

        [Fact]
        public async Task ConvertDocumentToTextAsync_WithConfig()
        {
            var service = new DocumentConversionService(Settings.Username, Settings.Password);
            string text;

            var config = JObject.Parse("{\"normalize_html\":{\"exclude_tags_completely\":[\"p\"]}}");

            using (var fs = new FileStream("SampleContent/html.html", FileMode.Open))
            {
                text = await service.ConvertDocumentToTextAsync(fs, FileType.Html, config);
            }

            Assert.False(string.IsNullOrWhiteSpace(text));
            Assert.False(text?.Contains("Para 1"));
        }
    }
}