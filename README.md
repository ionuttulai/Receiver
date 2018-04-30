# Receiver

There are two processors implemented implementing **IReceiverProcessor** interface. Both implementations focus only on re-ordering and processing packages in correct order. The communication, package content and acutal processing are mocked in this implementation.

### Processor.cs

**HandlePackage** method saves the incoming package into a buffer and reads all the ordered packages ready to process. Only processes packages when the incoming package is the next one in order.

### Processor2.cs

**HandlePackage** method saves the incoming package into a buffer.
A separate task reads coninuously from the buffer and processes ordered packages.
