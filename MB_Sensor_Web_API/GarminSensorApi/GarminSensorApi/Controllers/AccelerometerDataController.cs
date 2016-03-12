using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using GarminSensorApi.Models.SensorModels;
using GarminSensorApi.Utilities;

namespace GarminSensorApi.Controllers
{
    public class AccelerometerDataController : ApiController
    {
        private readonly IRepository<AccelerationBatch> m_accelerationDataRepository; 

        public AccelerometerDataController()
        {
            m_accelerationDataRepository = new Repository<AccelerationBatch>();
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [ResponseType(typeof(AccelerationBatch))]
        public IHttpActionResult Post([FromBody]AccelerationBatch accelerationBatch)
        {
            if (accelerationBatch == null)
            {
                return BadRequest("Invalid passed data.");
            }

            if (m_accelerationDataRepository.Add(accelerationBatch))
            {
                return Ok();
            }

            return new InternalServerErrorResult(new HttpRequestMessage());
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}