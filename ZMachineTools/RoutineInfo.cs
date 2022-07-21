namespace ZMachineTools
{
    internal record RoutineInfo(int addressStart, int lastInstruction, int addressError, int arguments, bool isParsed)
    {
    }

    internal record RoutineLayout(int addressStart, int lastInstruction, int arguments, List<string> disassembly);
}
