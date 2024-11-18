using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Buffers;
using System.Text;

public class MllpConnectionHandler : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        while (true)
        {
            // Read the next frame
            var result = await connection.Transport.Input.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer; 

            while (TryReadMllpFrame(ref buffer, out ReadOnlySequence<byte> frame))
            {
                // Process the MLLP frame
                ProcessHl7Message(frame);
            }

            // Tell the PipeReader how much of the buffer has been consumed.
            connection.Transport.Input.AdvanceTo(buffer.Start, buffer.End);

            // Stop reading   
            //if there's no more data coming.
            if (result.IsCompleted)
            {
                break;

            }
        }
        await connection.Transport.Input.CompleteAsync();
        Console.WriteLine($" {connection.RemoteEndPoint}: disconnected");

    }

    private static bool TryReadMllpFrame(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> frame)
    {
        var start = buffer.PositionOf((byte)0x0B);
        var end = buffer.PositionOf((byte)0x1C);
        if ( start != null && end != null )
        {
            frame = buffer.Slice(start.Value,buffer.GetPosition(1,end.Value));
            buffer = buffer.Slice(buffer.GetPosition(1,end.Value));
            return true;
        }
        frame = default;
        return false;
    }
    //private static bool TryReadMllpFrame1(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> frame)
    //{
    //    // Look for the Start-of-Block (SOB) byte
    //    if (buffer.Length == 0 || buffer.FirstSpan[0] != 0x0B)
    //    {
    //        frame = default;
    //        return false;
    //    }

    //    // Look for the End-of-Block (EOB) byte
    //    SequencePosition? position = buffer.PositionOf((byte)'\r'); // Use \r for EOB

    //    if (position == null)
    //    {
    //        frame = default;
    //        return false;
    //    }

    //    // Extract the MLLP frame (excluding SOB and EOB)
    //    frame = buffer.Slice(1, position.Value); // Skip SOB

    //    // Update the buffer to remove the processed data
    //    //buffer = buffer.Slice(buffer.GetPosition(1, position.Value) + 1); // Skip EOB too
    //    buffer = buffer.Slice(buffer.GetPosition(1, position.Value)); // Skip EOB too

    //    return true;
    //}
    private static void ProcessHl7Message(ReadOnlySequence<byte> message)
    {
        // Implement your HL7 message processing logic here
        // For example, parse the message, validate it, and send a response
        var strMessage = Encoding.UTF8.GetString(message);
        var parts = strMessage.Split('\r');
        foreach ( var part in parts ) {
            Console.WriteLine(part);
        }
        Console.WriteLine($"Received HL7 message: {strMessage}");
    }
}