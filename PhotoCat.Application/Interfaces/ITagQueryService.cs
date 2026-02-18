using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Application.Interfaces
{
    public interface ITagQueryService
    {
        Task<List<string>> GetDistinctTagsAsync(string? search, int limit, CancellationToken ct);
        //TODO: Implement this
    }
}
