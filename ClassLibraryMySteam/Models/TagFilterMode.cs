using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryMySteam.Models
{
    /// <summary>
    /// Логический режим фильтрации по тегам
    /// </summary>
    public enum TagFilterMode
    {
        Not = 1,
        Or = 2,
        And = 3
    }
}
