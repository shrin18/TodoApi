using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorValue> SensorValues { get; set; }
    }

    public class Node
    {
        // auto-incremented
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class Sensor
    {
        public long SensorId { get; set; }
        public long NodeId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
    }

    public class SensorValue
    {
        public long Id { get; set; }
        public long SensorId { get; set; }
        public long Timestamp { get; set; }
        public string Value { get; set; }
    }
}