using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Models
{
    public class ResultStats : ReactiveObject
    {
        public ResultStats(string status, string filesAnalyzed, bool isReady, int pagesAnalyzed, string executionTime)
        {
            Status = status;
            FilesAnalyzed = filesAnalyzed;
            IsReady = isReady;
            PagesAnalyzed = pagesAnalyzed;
            ExecutionTime = executionTime;
        }

        [Reactive]
        public string Status { get; set; }
        [Reactive]
        public string FilesAnalyzed { get; set; }
        [Reactive]
        public bool IsReady { get; set; }
        [Reactive]
        public int PagesAnalyzed { get; set; }
        [Reactive]
        public string ExecutionTime { get; set; }
    }
}
