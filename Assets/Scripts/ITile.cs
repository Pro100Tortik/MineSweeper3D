namespace MineSweeper
{
    public interface ITile
    {
        bool Dig();
        bool Flag(out bool placed, out bool removed);
    }
}
