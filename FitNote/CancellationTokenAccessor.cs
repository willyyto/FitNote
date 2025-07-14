namespace FitNote;

public interface ICancellationTokenAccessor {
  CancellationToken Token { get; }
}

public class CancellationTokenAccessor : ICancellationTokenAccessor {
  public CancellationToken Token => new CancellationTokenSource().Token;
}