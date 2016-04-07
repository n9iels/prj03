using System;
using System.Collections.Generic;
using System.Threading;

namespace TwitterApi.Queues.Helpers {
    internal abstract class QueueBase<T> : IDisposable {

        private readonly Queue<Action> _queue = new Queue<Action>();

        readonly ManualResetEvent _newItem = new ManualResetEvent(false);
        private readonly ManualResetEvent _waiting = new ManualResetEvent(false);
        private readonly ManualResetEvent _terminate = new ManualResetEvent(false);
               
        private readonly Thread _processingThread;
              
        protected QueueBase() {
            _processingThread = new Thread(ProcessQueue) { IsBackground = true };
            _processingThread.Start();
        }

        internal void Enqueue(T data) {
            lock (_queue)
                _queue.Enqueue(() => ProcessData(data));
            _newItem.Set();
        }

        protected abstract void ProcessData(T data);


        private void ProcessQueue() {
            while (true) {
                _waiting.Set();
                int index = WaitHandle.WaitAny(new WaitHandle[] { _newItem, _terminate });

                if (index == 1) // Terminate waithandle called. Exit method.
                    return;

                Queue<Action> queueCopy;
                lock (_queue) {
                    queueCopy = new Queue<Action>(_queue);
                    _queue.Clear();
                }
                _newItem.Reset();
                _waiting.Reset();

                int queueLength = queueCopy.Count;
                for (int i = 0; i < queueLength; i++)
                    queueCopy.Dequeue().Invoke();
            }
        }

        #region IDisposable Implementation Members

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _terminate.Set();
                _processingThread.Join();
            }
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
