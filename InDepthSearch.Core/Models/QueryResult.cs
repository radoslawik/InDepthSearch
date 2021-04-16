using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InDepthSearch.Core.Types;

namespace InDepthSearch.Core.Models
{
    public class QueryResult
    {
        public QueryResult()
        {
            MatchScale = MatchConfidence.High;
            FilePath = "";
            FileName = "";
            TextBefore = "";
            TextFound = "";
            TextAfter = "";
            PageNumber = 0;
        }

        public QueryResult(MatchConfidence matchScale, string filePath, string textBefore, string textFound, string textAfter, int pageNumber)
        {
            MatchScale = matchScale;
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            TextBefore = textBefore;
            TextFound = textFound;
            TextAfter = textAfter;
            PageNumber = pageNumber;
        }

        public MatchConfidence MatchScale { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string TextBefore { get; set; }
        public string TextFound { get; set; }
        public string TextAfter { get; set; }
        public int PageNumber { get; set; }

    }
}
