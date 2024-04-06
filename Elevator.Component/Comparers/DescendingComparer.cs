namespace Elevator.Component.Comparers;

internal class DescendingComparer : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return y.CompareTo(x);
    }
}