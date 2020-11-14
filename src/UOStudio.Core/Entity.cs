namespace UOStudio.Core
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public abstract void CopyFrom(Entity entity);
    }
}
