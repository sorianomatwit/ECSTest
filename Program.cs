using ECS;

public class Program
{
    struct Vector2
    {
        public int x;
        public int y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Position { }
    static Entity myEntity = new Entity(1, 0);
    public static void gameloop()
    {


        gameloop();
    }
    public static void Main(string[] args)
    {
        ComponentManager.CreateComponentSet<Position>();
        Entity e = EntityManager.CreateEntity();
        for (int i = 0; i < 5; i++)
        {
            
        }
        e.AddComponent<Position>(new Position());
    }
}

