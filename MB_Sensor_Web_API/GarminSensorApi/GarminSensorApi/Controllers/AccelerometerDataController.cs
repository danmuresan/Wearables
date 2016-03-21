using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using GarminSensorApi.Models.SensorModels;
using GarminSensorApi.Utilities;

namespace GarminSensorApi.Controllers
{
    public class AccelerometerDataController : DataController<AccelerationBatch>
    {
        private readonly IRepository<AccelerationBatch> m_accelerationDataRepository; 

        public AccelerometerDataController()
        {
            m_accelerationDataRepository = new Repository<AccelerationBatch>();
        }

        // GET api/values
        //[ResponseType(typeof(AccelerationBatch))]
        //public override IEnumerable<AccelerationBatch> Get()
        //{
        //    throw new NotImplementedException();
        //}

        // GET api/values/5
        public override AccelerationBatch Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/values
        [HttpPost]
        [ResponseType(typeof(AccelerationBatch))]
        public override IHttpActionResult Post([FromBody]AccelerationBatch accelerationBatch)
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
        public override IHttpActionResult Put(int id, [FromBody]AccelerationBatch value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/values/5
        public override IHttpActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}