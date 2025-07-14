namespace FitNote.Core.Dtos;

public record ValidationResultDto(
  bool IsValid,
  List<ValidationErrorDto> Errors
);