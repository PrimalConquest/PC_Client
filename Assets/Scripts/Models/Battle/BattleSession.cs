public static class BattleSession
{
    public static string Ip   { get; private set; } = "";
    public static int    Port { get; private set; }

    public static void Set(string ip, int port) { Ip = ip; Port = port; }
    public static void Clear() { Ip = ""; Port = 0; }
}
