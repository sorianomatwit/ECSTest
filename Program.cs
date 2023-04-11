public class Program
{
    public static void Main()
    {
        // Create a set set1 with capacity 5 and max
        // value 100
        SSet s1 = new SSet(100, 5);

        // Insert elements into the set set1
        s1.Insert(5);
        s1.Insert(3);
        s1.Insert(9);
        s1.Insert(10);

        Console.WriteLine(s1.dense[s1.sparse[5]] == 5);
        Console.WriteLine(s1.sparse[3]);
        // Printing the elements in the data structure.
        Console.WriteLine("The elements in set1 are");
        s1.Print();
        

        Console.WriteLine();
        int index = s1.Search(3);

        // 'index' variable stores the index of the number to
        // be searched.
        if (index != -1) // 3 exists
            Console.WriteLine("\n3 is found at index {0} in set1", index);
        else         // 3 doesn't exist
            Console.WriteLine("\n3 doesn't exists in set1");

        // Delete 9 and print set1
        s1.Deletion(9);
        s1.Print();

        // Create a set with capacity 6 and max value
        // 1000
        SSet s2 = new SSet(1000, 6);

        // Insert elements into the set
        s2.Insert(4);
        s2.Insert(3);
        s2.Insert(7);
        s2.Insert(200);

        // Printing set 2.
        Console.WriteLine("\nThe elements in set2 are");
        s2.Print();

        // Printing the intersection of the two sets
        SSet intersect = s2.Intersection(s1);
        Console.WriteLine("\nIntersection of set1 and set2");
        intersect.Print();

        // Printing the union of the two sets
        SSet unionset = s1.SetUnion(s2);
        Console.WriteLine("\nUnion of set1 and set2");
        unionset.Print();
    }
}