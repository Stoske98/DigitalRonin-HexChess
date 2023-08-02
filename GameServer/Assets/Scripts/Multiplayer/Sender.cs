using Riptide;

public static class Sender
{
    #region Core
    private static void SendReliableData(ushort _client_id, NetMessage _net_message)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)_net_message.op_code);
        _net_message.Serialize(message);
        NetworkManager.Instance.Server.Send(message, _client_id);
    }
    private static void SendReliableDataToAll(NetMessage _net_message)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)_net_message.op_code);
        _net_message.Serialize(message);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    private static void SendReliableDataToAllExceptTheGivenOne(ushort _client_id, NetMessage _net_message)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)_net_message.op_code);
        _net_message.Serialize(message);
        NetworkManager.Instance.Server.SendToAll(message, _client_id);
    }
    private static void SendUnreliableData(ushort _client_id, NetMessage _net_message)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)_net_message.op_code);
        _net_message.Serialize(message);
        NetworkManager.Instance.Server.Send(message, _client_id);
    }
    private static void SendUnreliableDataToAll(NetMessage _net_message)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)_net_message.op_code);
        _net_message.Serialize(message);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    private static void SendUnreliableDataToAllExceptTheGivenOne(ushort _client_id, NetMessage _net_message)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)_net_message.op_code);
        _net_message.Serialize(message);
        NetworkManager.Instance.Server.SendToAll(message, _client_id);
    }
    #endregion

    #region Reliable
    public static void SendToClient_Reliable(ushort _client_id, NetMessage _net_message)
    {
        SendReliableData(_client_id, _net_message);
    }

    public static void SendToAllClients_Reliable(NetMessage _net_message)
    {
        SendReliableDataToAll(_net_message);
    }
    public static void SendToAllClientsExceptToGivenOne_Reliable(ushort _excluded_client_id, NetMessage _net_message)
    {
        SendReliableDataToAllExceptTheGivenOne(_excluded_client_id, _net_message);
    }
    #endregion
    #region Unreliable
    public static void SendToClient_Unreliable(ushort _client_id, NetMessage _net_message)
    {
        SendUnreliableData(_client_id, _net_message);
    }
    public static void SendToAllClients_Unreliable(NetMessage _net_message)
    {
        SendUnreliableDataToAll(_net_message);
    }
    public static void SendToAllClientsExceptToGivenOne_Unreliable(ushort _excluded_client_id, NetMessage _net_message)
    {
        SendUnreliableDataToAllExceptTheGivenOne(_excluded_client_id, _net_message);
    }
    #endregion
}

