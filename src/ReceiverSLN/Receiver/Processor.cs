using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Receiver
{
    internal class Processor : IReceiverProcessor, IDisposable
    {
        private static long _nextSequenceToProcess;
        private readonly Dictionary<long, object> _buffer;

        public Processor(long currentSequence)
        {
            //initialize with next expected value
            _nextSequenceToProcess = currentSequence + 1;
            _buffer = new Dictionary<long, object>();
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

            //read a sequence of consecutive packages ready to be processed in order
            lock (_buffer)
            {
                while (_buffer.TryGetValue(_nextSequenceToProcess, out var pk))
                {
                    /* process package and send response to sender 
                     * in case of an error break out of the loop and keep current package as first to process when next one comes in. 
                     * a more elaborate retry mechanism can be implemented here. In the current implementation it will only retry when another message is received */
                    if (!ProcessPackage(pk))
                        break;

                    /* cleanup could be implemented on a different thread for all keys smaller than current
                     * once _next sequence is incremented the current value will be ignored */
                    _buffer.Remove(_nextSequenceToProcess);
                    _nextSequenceToProcess++;
                }
            }
        }

        /* Mocked processing method  */
        private bool ProcessPackage(object package)
        {
            Debug.WriteLine($"Processing {_nextSequenceToProcess}");
            return true;
        }

        public void Dispose()
        {
            lock (_buffer)
            {
                _buffer.Clear();
            }
        }
    }


}
