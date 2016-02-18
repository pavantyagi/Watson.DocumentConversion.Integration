using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            //new object[] {"doc.doc", FileType.MsWordDoc},
            //new object[] {"docx.docx", FileType.MsWordDoc},
            //new object[] {"pdf.pdf", FileType.Pdf},
            //new object[] {"xml.xml", FileType.Xml}
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
            IAnswers answers = null;

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                answers = await service.ConvertDocumentToAnswersAsync(fs, fileType);
            }

            Assert.NotNull(answers);
            Assert.True(answers.AnswerUnits.Any());
        }
    }
}