using System;
using System.Diagnostics.CodeAnalysis;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatsDoorman
    {
        bool TryLockChat(long chatId, [NotNullWhen(true)] out IDisposable? chatLock);
    }
}