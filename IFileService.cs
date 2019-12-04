using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextComparer
{
    public interface IFileService
    {
        Task<Text> OpenAsync(string filename);
    }
}
