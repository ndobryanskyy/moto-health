using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MotoHealth.Core.Bot;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class ChatsDoormanTests
    {
        private readonly InMemoryChatsDoorman _chatsDoorman;

        public ChatsDoormanTests()
        {
            _chatsDoorman = new InMemoryChatsDoorman(Mock.Of<ILogger<InMemoryChatsDoorman>>());
        }

        [Fact]
        public void Should_Return_ChatLock_When_Chat_Is_Locked()
        {
            var lockAcquired = _chatsDoorman.TryLockChat(1, out var chatLock);

            lockAcquired.Should().BeTrue();
            chatLock.Should().NotBeNull();
        }

        [Fact]
        public void Should_Allow_Locking_Different_Chats_Simultaneously()
        {
            var firstChatLockAcquired = _chatsDoorman.TryLockChat(1, out var firstChatLock);
            var secondChatLockAcquired = _chatsDoorman.TryLockChat(2, out var secondChatLock);

            firstChatLockAcquired.Should().BeTrue();
            firstChatLock.Should().NotBeNull();

            secondChatLockAcquired.Should().BeTrue();
            secondChatLock.Should().NotBeNull();
        }

        [Fact]
        public void Should_Not_Lock_Already_Locked_Chat()
        {
            _chatsDoorman.TryLockChat(1, out var successfulLock);

            using (successfulLock)
            {
                var secondAttemptLocked = _chatsDoorman.TryLockChat(1, out var secondAttemptChatLock);

                secondAttemptLocked.Should().BeFalse();
                secondAttemptChatLock.Should().BeNull();
            }
        }

        [Fact]
        public void Should_Allow_Locking_The_Same_Chat_After_It_Was_Unlocked()
        {
            if (_chatsDoorman.TryLockChat(1, out var firstAttemptChatLock))
            {
                firstAttemptChatLock.Dispose();
            }

            var lockAcquired = _chatsDoorman.TryLockChat(1, out var secondAttemptChatLock);

            lockAcquired.Should().BeTrue();
            secondAttemptChatLock.Should().NotBeNull();
        }

        [Fact]
        public void Should_Allow_Disposing_ChatLock_More_Than_Once_Without_Exceptions()
        {
            if (_chatsDoorman.TryLockChat(1, out var chatLock))
            {

                chatLock.Invoking(x =>
                {
                    x.Dispose();
                    x.Dispose();
                    x.Dispose();
                }).Should().NotThrow();
            }
        }

        [Fact]
        public void Should_Handle_Parallel_Locking_And_Unlocking_Of_Different_Chats()
        {
            var chatIds = Enumerable.Range(1, 100).ToArray();
         
            var locksAcquired = 0;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            Parallel.ForEach(chatIds, parallelOptions, chatId =>
            {
                if (_chatsDoorman.TryLockChat(chatId, out var chatLock))
                {
                    using (chatLock)
                    {
                        Interlocked.Increment(ref locksAcquired);
                    }
                }
            });

            locksAcquired.Should().Be(chatIds.Length);

            var locksReleased = 0;

            foreach (var chatId in chatIds)
            {
                if (_chatsDoorman.TryLockChat(chatId, out _))
                {
                    locksReleased++;
                }
            }

            locksReleased.Should().Be(locksAcquired);
        }

        [Fact]
        public void Should_Lock_Chat_Only_Once_Even_In_Parallel_Scenario()
        {
            var locksAcquired = 0;

            const int degreeOfParallelism = 10;
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = degreeOfParallelism
            };

            Parallel.For(1, degreeOfParallelism, parallelOptions, x =>
            {
                if (_chatsDoorman.TryLockChat(1, out _))
                {
                    Interlocked.Increment(ref locksAcquired);
                }
            });

            locksAcquired.Should().Be(1);
        }
    }
}