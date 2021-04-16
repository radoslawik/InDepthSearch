using InDepthSearch.Core.Services;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.Core.Types;
using Xunit;

namespace InDepthSearch.Tests.Services
{
    public class OptionServiceTest
    {
        private readonly IOptionService _optionService;

        public OptionServiceTest()
        {
            _optionService = new OptionService();
        }

        [Fact]
        public void TranslateLanguageTest()
        {
            Assert.Equal("eng", _optionService.TranslateLanguage(RecognitionLanguage.Default));
        }
    }
}

