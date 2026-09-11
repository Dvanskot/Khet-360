using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IFamilyPortalService
{
    Task<TokenResponseDto> GenerateCaseAccessTokenAsync(Guid caseId);
    Task<FamilyCaseViewDto?> GetCaseViewByTokenAsync(string token);
    Task<Guid> UploadDocumentAsync(string token, System.IO.Stream fileStream, string fileName, string contentType, Guid documentRequestId);
}
