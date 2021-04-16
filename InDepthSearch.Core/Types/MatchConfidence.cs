using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Types
{
    public enum MatchConfidence : int
    {
        High, // found exact expression
        Medium, // found the all the words from the expression
        Low // found part of the words from the expression
    }

}
