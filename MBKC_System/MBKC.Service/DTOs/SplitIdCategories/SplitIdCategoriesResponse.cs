using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.SplitIdCategories
{
    public class SplitIdCategoryResponse
    {
        public List<int> idsToRemove { get; set; }
        public List<int> idsToAdd { get; set; }
        public List<int> idsToKeep { get; set; }
    }
}
