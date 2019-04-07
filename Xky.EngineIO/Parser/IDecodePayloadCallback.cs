
namespace Xky.EngineIO.Parser
{

    public interface IDecodePayloadCallback
    {
         bool Call(Packet packet, int index, int total);
    }
}