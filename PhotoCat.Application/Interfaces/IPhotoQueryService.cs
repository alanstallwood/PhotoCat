using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Application.Interfaces
{
    public interface IPhotoQueryService
    {
        //TODO: Define query methods for fetching photos with various filters, pagination, etc.
        //Task<PagedResult<PhotoSummaryDto>> GetPagedAsync(PhotoSearchCriteria criteria, int pageSize, string continuationToken, CancellationToken ct);
    }
}
