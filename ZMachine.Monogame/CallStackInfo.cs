namespace ZMachine.Monogame
{
    internal record ActivationRecord(int returnAdrress, int startAdress, byte[] locals) { };
}