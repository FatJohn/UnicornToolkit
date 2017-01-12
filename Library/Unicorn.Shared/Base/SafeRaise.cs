// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Unicorn
{
    public static class SafeRaise
    {
        /// <summary>
        /// Raises an event in a thread-safe manner, also does the null check.
        /// </summary>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        public static void Raise(EventHandler eventToRaise, object sender)
        {
            eventToRaise?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises an event in a thread-safe manner, also does the null check.
        /// </summary>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        public static void Raise(EventHandler<EventArgs> eventToRaise, object sender)
        {
            Raise(eventToRaise, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises an event in a thread-safe manner, also does the null check.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event args.</param>
        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, T args)
        {
            eventToRaise?.Invoke(sender, args);
        }

        // Lazy event args creation example:
        //
        // public class MyEventArgs : EventArgs
        // {
        //     public MyEventArgs(int x) { X = x; }
        //     public int X { get; set; }
        // }
        //
        // event EventHandler<MyEventArgs> Foo;
        //
        // public void Bar()
        // {
        //     int y = 2;
        //     Raise(Foo, null, () => { return new MyEventArgs(y); });
        // }

        /// <summary>
        /// This is a method that returns event args, used for lazy creation.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns></returns>
        public delegate T GetEventArgs<T>();

        /// <summary>
        /// Raise an event in a thread-safe manner, with the required null check. Lazily creates event args.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="getEventArgs">The delegate to return the event args if needed.</param>
        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, GetEventArgs<T> getEventArgs)
        {
            eventToRaise?.Invoke(sender, getEventArgs());
        }
    }
}
