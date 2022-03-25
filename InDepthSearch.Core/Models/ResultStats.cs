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
        public ResultStats()
        {
            FilesAnalyzed = "";
            PagesAnalyzed = 0;
            ExecutionTime = "";
        }

        [Reactive]
        public string FilesAnalyzed { get; set; }
        [Reactive]
        public int PagesAnalyzed { get; set; }
        [Reactive]
        public string ExecutionTime { get; set; }
    }
}
