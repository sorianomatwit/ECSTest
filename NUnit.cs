using System;
using NUnit.Framework;
using ECS;

public struct Test { }
[TestFixture]
public class AllTest
{
    [Test]
    public void HasEntityTest() {
        Entity e = EntityManager.CreateEntity();
        ComponentSet<Test> bruh = ComponentManager.CreateComponentSet<Test>();
        bruh.AddEntity(e, new Test());
        Assert.True(bruh.HasEntity(e));
    }
}