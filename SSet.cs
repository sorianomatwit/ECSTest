using System;

internal class SSet
{
	public int[] sparse;
	public int[] dense;
	private int n;
	private int capacity;
	private int maxValue;

	public SSet(int maxV, int cap)
	{
		sparse = new int[maxV + 1];
		dense = new int[cap];
		capacity = cap;
		maxValue = maxV;
		n = 0;
	}

	public int Search(int x)
	{
		if (x > maxValue)
			return -1;

		if (sparse[x] < n && dense[sparse[x]] == x)
			return (sparse[x]);

		return -1;
	}

	public void Insert(int x)
	{
		if (n == capacity)
			return;

		if (x > maxValue)
			return;

		int i = sparse[x];
		if (i >= n || dense[i] != x)
		{
			dense[n] = x;
			sparse[x] = n;
			n++;
		}
	}

	public void Deletion(int x)
	{
		if (x > maxValue)
			return;

		int i = sparse[x];
		if (i < n && dense[i] == x)
		{
			n--;
			dense[i] = dense[n];
			sparse[dense[n]] = i;
		}
	}

	public void Print()
	{
		Console.Write("Sparse Set: ");
		for (int i = 0; i < n; i++)
			Console.Write(dense[i] + " ");
		Console.WriteLine();
	}

	public void Clear()
	{
		n = 0;
	}

	public SSet Intersection(SSet s)
	{
		SSet result = new SSet(maxValue, capacity);
		for (int i = 0; i < n; i++)
		{
			if (s.Search(dense[i]) != -1)
				result.Insert(dense[i]);
		}
		return result;
	}

	public SSet SetUnion(SSet s)
	{
		SSet result = new SSet(maxValue, capacity);
		for (int i = 0; i < n; i++)
			result.Insert(dense[i]);
		for (int i = 0; i < s.n; i++)
			result.Insert(s.dense[i]);
		return result;
	}
}


