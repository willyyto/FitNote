namespace FitNote.Core.Dtos;

public record ApiResponseDto<T>(
  bool Success,
  T? Data,
  string? Message,
  List<string>? Errors,
  int? StatusCode
);