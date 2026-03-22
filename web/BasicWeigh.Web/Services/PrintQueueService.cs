using System.Collections.Concurrent;

namespace BasicWeigh.Web.Services;

public class PrintQueueService
{
    private readonly ConcurrentQueue<string> _queue = new();

    public void Enqueue(string ticketId) => _queue.Enqueue(ticketId);
    public bool TryDequeue(out string? ticketId) => _queue.TryDequeue(out ticketId);
    public bool HasPending => !_queue.IsEmpty;
}
