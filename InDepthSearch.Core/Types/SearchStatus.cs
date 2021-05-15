using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Types
{
    public enum SearchStatus : int
    {
        Unknown,
        Ready,
        Initializing,
        Running,
    }
}
