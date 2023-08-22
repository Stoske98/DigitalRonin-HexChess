using Riptide;

public static class Sender
{
    #region Core
    private static void SendReliableData(NetMessage net_message)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)net_message.op_code);
        net_message.Serialize(message);
        NetworkManager.Instance.Client.Send(message);
    }

    private static void SendUnreliableData(NetMessage net_message)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)net_message.op_code);
        net_message.Serialize(message);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion
    #region Reliable
    public static void SendToServer_Reliable(NetMessage net_message)
    {
        SendReliableData(net_message);
    }
    #endregion
    #region Unreliable
    public static void SendToServer_Unreliable(NetMessage net_message)
    {
        SendUnreliableData(net_message);
    }
    #endregion
}
