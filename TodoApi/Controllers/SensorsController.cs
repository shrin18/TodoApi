using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class SensorsController : ControllerBase
    {
        public readonly TodoContext _db;
        public SensorsController(TodoContext database) {
            _db = database;
            if (_db.Sensors.Count() == 0) {
                _db.Sensors.Add(new Sensor
                {
                    NodeId = 1,
                    Type = 1,
                    Name = "Rainfall",
                    Description = "Default API server sensor"
                });
                _db.SaveChanges();
            }
        }



        // GET: api/<SensorsController>
        [HttpGet]
        public ActionResult<List<Sensor>> GetSensors()
        {
            return _db.Sensors.ToList();
        }

        // GET api/<SensorsController>/5
        [HttpGet("{id}")]
        public ActionResult<Sensor> GetSensor(long id)
        {
            var sensor = _db.Sensors.Find(id);
            if (sensor == null) return NotFound();
            return sensor;
        }

        // POST api/<SensorsController>
        [HttpPost]
        public ActionResult<Sensor> PostSensor([FromBody] Sensor sensor)
        {
            var exists = _db.Sensors.Where(o => o.Name == sensor.Name).Count();
            if (exists == 0)
            {
                if (_db.Nodes.Find(sensor.NodeId) == null) return BadRequest();
                _db.Sensors.Add(new Sensor
                {
                    Name = sensor.Name,
                    NodeId = sensor.NodeId,
                    Type = sensor.Type,
                    Description = sensor.Description
                });
                _db.SaveChanges();
                return _db.Sensors.Last();
            }
            else return BadRequest();
        }

        // PUT api/<SensorsController>/5
        [HttpPut("{id}")]
        public ActionResult<Sensor> PutSensor(long id, [FromBody] Sensor update)
        {
            var sensor = _db.Sensors.Find(id);
            if (sensor == null) return NotFound();
            if (_db.Nodes.Find(update.NodeId) == null) return BadRequest();
            sensor.NodeId = update.NodeId;
            sensor.Name = update.Name;
            sensor.Type = update.Type;
            sensor.Description = update.Description;
            _db.SaveChanges();
            return sensor;
        }
        // DELETE api/<SensorsController>/5
        [HttpDelete("{id}")]
        public ActionResult<Sensor> DeleteSensor(long id)
        {
            var sensor = _db.Sensors.Find(id);
            if (sensor == null) return NotFound();
            var deleted = new Sensor
            {
                SensorId = 0,
                NodeId = sensor.NodeId,
                Type = sensor.Type,
                Name = sensor.Name,
                Description = sensor.Description
            };
            _db.Remove(sensor);
            _db.SaveChanges();
            return deleted;
        }
    }
}
