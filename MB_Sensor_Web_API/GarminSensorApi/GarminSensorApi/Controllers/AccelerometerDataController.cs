using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using GarminSensorApi.Models.SensorModels;
using GarminSensorApi.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        //[ResponseType(typeof(AccelerationBatch))]
        //public override IEnumerable<AccelerationBatch> Get()
        //{
        //    throw new NotImplementedException();
        //}

        // GET api/values/5
        public AccelerationBatch Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/values
        [HttpPost]
        [ResponseType(typeof(AccelerationBatch))]
        public async Task<IHttpActionResult> Post(HttpRequestMessage request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            var data = await request.Content.ReadAsStringAsync();
            data = data.Replace("\"[", "[");
            data = data.Replace("]\"}", "]}");

            try
            {
                List<Acceleration> accelerations = JsonConvert.DeserializeObject<AccelerationDataBatch>(data).Accelerations;
                m_accelerationDataRepository.Add(new AccelerationBatch { AccelerationList = accelerations, TimeStamp = DateTime.Now });
            }
            catch (Exception ex)
            {
                // TODO

                return new InternalServerErrorResult(new HttpRequestMessage());
            }

            return Ok();
        }

        // PUT api/values/5
        public IHttpActionResult Put(int id, [FromBody]AccelerationBatch value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/values/5
        public IHttpActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}