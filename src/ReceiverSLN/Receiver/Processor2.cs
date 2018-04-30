using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Receiver
{
    internal class Processor2 : IReceiverProcessor, IDisposable
    {
        private  long _nextSequenceToProcess;
        private readonly Dictionary<long, object> _buffer;
        private readonly CancellationTokenSource _cts;

        public Processor2(long currentSequence)
        {
            _nextSequenceToProcess = currentSequence + 1;
            _buffer = new Dictionary<long, object>();
            _cts = new CancellationTokenSource();

            StartReadingTask();
        }

        public void HandlePackage(object package, long sequence)
        {
            if (package == null)
                return;

            //add the package to the internal buffer or update if the package was resentby the sender
            lock (_buffer)
            {
                if (_buffer.ContainsKey(sequence))
                    _buffer[sequence] = package;
                else
                    _buffer.Add(sequence, package);
            }
        }

        /*Read from the buffer Continuously on a different task */
        private void StartReadingTask()
        {
            var ct = _cts.Token;
            Task.Run(() =>
            {
                //this could be implemented with a timer/event mechanism
                while (true)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    lock (_buffer)
                    {
                        while (_buffer.TryGetValue(_nextSequenceToProcess, out var pk))
                        {
                            /* process package 
                             * in case of an error break out of the loop and keep current package as first to process for infinit retries 
                             * a better retry mechanism/error handling could be implemented here */ 
                            if (!ProcessPackage(pk))
                                break;

                            _buffer.Remove(_nextSequenceToProcess);
                            _nextSequenceToProcess++;
                        }
                    }
                }
            }, ct);
        }

        /* Mocked processing method  */
        private bool ProcessPackage(object package)
        {
            Debug.WriteLine($"Processing {_nextSequenceToProcess}");

            return true;
        }

        public void Dispose()
        {
            _cts?.Cancel();
            lock (_buffer)
            {
                _buffer.Clear();
            }
        }
    }
}
