using AutoMapper;
using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Entities;
using FitNote.Core.Enums;
using FitNote.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FitNote.Application.Services;

public class ExerciseService : IExerciseService {
  private readonly ILogger<ExerciseService> _logger;
  private readonly IMapper _mapper;
  private readonly IUnitOfWork _unitOfWork;

  public ExerciseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ExerciseService> logger) {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _logger = logger;
  }

  public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid id) {
    var exercise = await _unitOfWork.Exercises.GetByIdAsync(id);
    return exercise != null ? _mapper.Map<ExerciseDto>(exercise) : null;
  }

  public async Task<IEnumerable<ExerciseDto>> GetAllExercisesAsync() {
    var exercises = await _unitOfWork.Exercises.GetAllAsync();
    return _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
  }

  public async Task<IEnumerable<ExerciseDto>> GetExercisesByCategoryAsync(ExerciseCategory category) {
    var exercises = await _unitOfWork.Exercises.GetExercisesByCategoryAsync(category);
    return _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
  }

  public async Task<IEnumerable<ExerciseDto>> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup) {
    var exercises = await _unitOfWork.Exercises.GetExercisesByMuscleGroupAsync(muscleGroup);
    return _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
  }

  public async Task<IEnumerable<ExerciseDto>> GetUserExercisesAsync(Guid userId) {
    var exercises = await _unitOfWork.Exercises.GetUserExercisesAsync(userId);
    return _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
  }

  public async Task<IEnumerable<ExerciseDto>> GetDefaultExercisesAsync() {
    var exercises = await _unitOfWork.Exercises.GetDefaultExercisesAsync();
    return _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
  }

  public async Task<IEnumerable<ExerciseDto>> SearchExercisesAsync(string searchTerm) {
    var exercises = await _unitOfWork.Exercises.SearchExercisesAsync(searchTerm);
    return _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
  }

  public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseInput input, Guid userId) {
    var exercise = _mapper.Map<Exercise>(input);
    exercise.Id = Guid.NewGuid();
    exercise.CreatedByUserId = userId;

    await _unitOfWork.Exercises.AddAsync(exercise);
    await _unitOfWork.SaveChangesAsync();

    var createdExercise = await _unitOfWork.Exercises.GetByIdAsync(exercise.Id);
    return _mapper.Map<ExerciseDto>(createdExercise);
  }

  public async Task<bool> DeleteExerciseAsync(Guid id, Guid userId) {
    var exercise = await _unitOfWork.Exercises.GetByIdAsync(id);
    if (exercise == null || (exercise.CreatedByUserId != userId && !exercise.IsDefault))
      return false;

    // Check if exercise is used in any workouts
    var isUsed = await _unitOfWork.Repository<WorkoutExercise>()
      .ExistsAsync(we => we.ExerciseId == id);

    if (isUsed) {
      _logger.LogWarning("Attempted to delete exercise {ExerciseId} that is in use", id);
      return false;
    }

    _unitOfWork.Exercises.Delete(exercise);
    await _unitOfWork.SaveChangesAsync();
    return true;
  }
}