namespace ZMachineTools
{
    internal record RoutineInfo(int addressStart,  int addressError, int arguments,  bool isParsed);

    internal record RoutineLayout(int addressStart, int addressError, int arguments, List<string> disassembly);
}
