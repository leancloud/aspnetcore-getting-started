using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using web.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace web.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private IConnectionMultiplexer _connectionMultiplexer;
        public TodoController(IConnectionMultiplexer multiplexer)
        {
            this._connectionMultiplexer = multiplexer;
        }
        // GET: api/todo
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/todo/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            var json = db.StringGet(id.ToString());
            if (string.IsNullOrEmpty(json)) return Json("{}");
            var todo = Todo.FromJson(json);
            return Json(todo);
        }

        // POST api/todo
        [HttpPost]
        public JsonResult Post([FromBody]Todo todo)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();

            db.StringSet("1", todo.ToJson());

            return Json(new { ID = 1 });
        }

        // PUT api/todo/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Todo todo)
        {

        }

        // DELETE api/todo/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
