using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Application
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);
    }

}
