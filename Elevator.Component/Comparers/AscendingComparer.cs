namespace Elevator.Component.Comparers;

internal class AscendingComparer : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return x.CompareTo(y);
    }
}