using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
    public interface IArticleImageService
    {
        Task AddAsync(int id, List<string> imagePaths);
        Task ChangeDirectory(int articleId);
    }
}
