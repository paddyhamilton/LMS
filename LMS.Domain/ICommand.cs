using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Domain
{
    public interface ICommand
    {
        Guid Id { get;}
    }

    public class Command : ICommand
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public Command(Guid id, int version)
        {
            Id = id;
            Version = version;
        }
    }

    public interface ICommandHandler<in TCommand> where TCommand : Command
    {
        void Execute(TCommand command);
    }

    public interface IEvent
    {
        Guid Id { get; }
    }

    public class Event : IEvent
    {
        public Event()
        {
            Id = Guid.NewGuid();
        }

        public int Version;
        public Guid AggregateId { get; set; }
        public Guid Id { get; private set; }
    }

    public interface IEventProvider
    {
        void LoadsFromHistory(IEnumerable<Event> history);
        IEnumerable<Event> GetUncommittedChanges();
    }

    public abstract class AggregateRoot : IEventProvider
    {
        private readonly List<Event> _changes;

        public Guid Id { get; internal set; }
        public int Version { get; internal set; }
        public int EventVersion { get; protected set; }

        protected AggregateRoot()
        {
            _changes = new List<Event>();
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) Apply(e, false);
            Version = history.Last().Version;
            EventVersion = Version;
        }

        protected void Apply(Event @event)
        {
            Apply(@event, true);
        }

        private void Apply(Event @event, bool isNew)
        {
            dynamic d = this;
            var eventType = @event.GetType();
            try
            {
                d.Handle(Converter.ChangeTo(@event, eventType));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("No handler specified for Event {0}", eventType.Name));
            }
          
            if (isNew)
            {
                _changes.Add(@event);
            }
        }
    }

    public static class Converter
    {
        public static Action<object> Convert<T>(Action<T> myActionT)
        {
            if (myActionT == null) return null;
            return o => myActionT((T)o);
        }

        public static dynamic ChangeTo(dynamic source, Type dest)
        {
            return System.Convert.ChangeType(source, dest);
        }
    }
}
