public class Pair<T>
{
    public T first { get; private set; }
    public T second { get; private set; }

    public Pair(T a, T b)
    {
        this.first = a;
        this.second = b;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        Pair<T> other = (Pair<T>)obj;
        return (first.Equals(other.first) && second.Equals(other.second)) ||
            (first.Equals(other.second) && (second.Equals(other.first)));
    }

    public override int GetHashCode()
    {
        int firstHash = first == null ? 0 : first.GetHashCode();
        int secondHash = second == null ? 0 : second.GetHashCode();
        return firstHash ^ secondHash;
    }
}