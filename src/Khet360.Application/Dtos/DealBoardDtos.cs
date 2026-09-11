using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record DealBoardDto(
    string BoardName,
    List<DealColumnDto> Columns);

public record DealColumnDto(
    string StageName,
    int StageValue,
    List<DealCardDto> Cards);

public record DealCardDto(
    Guid Id,
    string Title,
    string Subtitle,
    decimal? Value,
    DateTime CreatedAt,
    string Status);
