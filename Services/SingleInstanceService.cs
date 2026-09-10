using System.Threading;

namespace MoveBreak.Services;

public sealed class SingleInstanceService : IDisposable
{
    private const string MutexName = @"Local\MoveBreak.SingleInstance.v1";
    private const string ActivationEventName = @"Local\MoveBreak.Activate.v1";

    private readonly Mutex _mutex;
    private readonly EventWaitHandle _activationEvent;
    private readonly bool _ownsMutex;
    private RegisteredWaitHandle? _activationRegistration;

    public SingleInstanceService()
    {
        // Create the event first so a secondary process can always signal the
        // primary process, even while the primary process is still starting.
        _activationEvent = new EventWaitHandle(
            false,
            EventResetMode.AutoReset,
            ActivationEventName);

        _mutex = new Mutex(true, MutexName, out _ownsMutex);
        IsPrimaryInstance = _ownsMutex;

        if (!IsPrimaryInstance)
        {
            _activationEvent.Set();
        }
    }

    public bool IsPrimaryInstance { get; }

    public void ListenForActivation(Action activationRequested)
    {
        if (!IsPrimaryInstance)
        {
            throw new InvalidOperationException("Only the primary instance can listen for activation.");
        }

        _activationRegistration = ThreadPool.RegisterWaitForSingleObject(
            _activationEvent,
            (_, _) => activationRequested(),
            null,
            Timeout.Infinite,
            executeOnlyOnce: false);
    }

    public void Dispose()
    {
        _activationRegistration?.Unregister(null);
        _activationEvent.Dispose();

        if (_ownsMutex)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (ApplicationException)
            {
                // The operating system also releases ownership when the
                // process exits, so disposal remains safe during shutdown.
            }
        }

        _mutex.Dispose();
    }
}
