using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> repository;

        public CommentService(IRepository<Comment> repository)
        {
            this.repository = repository;
        }


    }
}
