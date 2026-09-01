using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IFamilyPortalService
{
    Task<TokenResponseDto> GenerateCaseAccessTokenAsync(Guid caseId);
    Task<FamilyCaseViewDto?> GetCaseViewByTokenAsync(string token);
    Task<Guid> UploadDocumentAsync(string token, System.IO.Stream fileStream, string fileName, string contentType);
}
