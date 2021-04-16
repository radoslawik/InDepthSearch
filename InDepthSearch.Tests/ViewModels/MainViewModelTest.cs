using System;
using Xunit;
using InDepthSearch.Core.ViewModels;
using InDepthSearch.Core.Services;
using InDepthSearch.Core.Models;

namespace InDepthSearch.Tests.ViewModels
{
    public class MainViewModelTest
    {
        private readonly MainViewModel _mainViewModel;

        public MainViewModelTest()
        {
            _mainViewModel = new MainViewModel(new OptionService(), new DirectoryService()); // TODO implement mocked services
        }

        [Fact]
        public void ReadPdfTest()
        {
            _mainViewModel.Results.Add(new QueryResult());
            Assert.Single(_mainViewModel.Results);
            _mainViewModel.ReadPDF.Execute().Subscribe();
            Assert.Empty(_mainViewModel.Results);
        }
    }
}
