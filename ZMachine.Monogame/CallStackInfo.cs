namespace ZMachine.Monogame
{
    internal record CallStackInfo(int returnAdrress, int startAdress, byte[] locals) { };
}