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
    public class AccelerometerDataController : DataController<AccelerationBatch>
    {
        private readonly IRepository<AccelerationBatch> m_accelerationDataRepository; 

        public AccelerometerDataController()
        {
            m_accelerationDataRepository = new Repository<AccelerationBatch>();
        }

        // GET api/values/5
        [HttpGet]
        public override AccelerationBatch Get(int id)
        {
            return m_accelerationDataRepository.GetById(id);
            //return new AccelerationBatch()
            //{
            //    Accelerations = new List<Acceleration>(proxy.Accelerations),
            //    TimeStamp = proxy.TimeStamp,
            //    Id = proxy.Id
            //};
        }

        // POST api/values
        [HttpPost]
        [ResponseType(typeof(AccelerationBatch))]
        public override async Task<IHttpActionResult> Post(HttpRequestMessage request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            var data = await request.Content.ReadAsStringAsync();
            data = data.Replace("\"[", "[");
            data = data.Replace("]\"}", "]}");
            bool added = false;

            try
            {
                var accelerations = JsonConvert.DeserializeObject<AccelerationBatch>(data).Accelerations;
                added = m_accelerationDataRepository.Add(new AccelerationBatch { Accelerations = accelerations, TimeStamp = DateTime.Now });
            }
            catch (Exception ex)
            {
                // TODO

                //return new InternalServerErrorResult();
                throw;
            }

            if (added)
            {
                return Ok();
            }

            return new InternalServerErrorResult(new HttpRequestMessage());
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