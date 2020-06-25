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
    public class ValuesController : ControllerBase
    {
        private readonly TodoContext _db;
        public ValuesController(TodoContext database) {
            _db = database;
            if (_db.Nodes.Count() == 0)
            {
                _db.Nodes.Add(new Node
                {
                    Name = "Server",
                    Description = "Default API server node"
                });
                _db.SaveChanges();
            }


        }
        // GET: api/<ValuesController>
        [HttpGet]
        public ActionResult<List<Node>> GetNodes()
        {
            return _db.Nodes.ToList();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public ActionResult<Node> GetNode(long id)
        {
            var node = _db.Nodes.Find(id);
            if (node == null) return NotFound();
            return node;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public ActionResult<Node> PostNode([FromBody] Node node)
        {
            //var exists = (from x in _db.Nodes where x.Name == node.Name select x.Id).Count();
            var exists = _db.Nodes.Where(o => o.Name == node.Name).Count();
            if (exists == 0)
            {
                _db.Nodes.Add(new Node
                {
                    Name = node.Name,
                    Description = node.Description
                });
                _db.SaveChanges();
                return _db.Nodes.Last();
            }
            else return BadRequest();
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public ActionResult<Node> PutNode(long id, [FromBody] Node update)
        {
            var node = _db.Nodes.Find(id);
            if (node == null) return NotFound();
            node.Name = update.Name;
            node.Description = update.Description;
            _db.SaveChanges();
            return node;
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public ActionResult<Node> DeleteNode(long id)
        {
            var node = _db.Nodes.Find(id);
            if (node == null) return NotFound();
            var deleted = new Node
            {
                Id = 0,
                Name = node.Name,
                Description = node.Description
            };
            _db.Remove(node);
            _db.SaveChanges();
            return deleted;
        }


    }
 
}
