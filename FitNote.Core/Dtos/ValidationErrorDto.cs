namespace FitNote.Core.Dtos;

public record ValidationErrorDto(
  string Field,
  string Message,
  string Code
);