using System.Collections.Generic;
using System.Web.Http;
using GarminSensorApi.Models;

namespace GarminSensorApi.Controllers
{
    public abstract class DataController<TEntity> : ApiController where TEntity : IDataTableModel
    {
        // GET api/values
        //[HttpGet]
        //public abstract IEnumerable<TEntity> Get();

        // GET api/values/5
        [HttpGet]
        public abstract TEntity Get(int id);

        // POST api/values
        [HttpPost]
        public abstract IHttpActionResult Post([FromBody] TEntity data);

        [HttpPut]
        // PUT api/values/5
        public abstract IHttpActionResult Put(int id, [FromBody] TEntity data);

        [HttpDelete]
        // DELETE api/values/5
        public abstract IHttpActionResult Delete(int id);
    }
}
