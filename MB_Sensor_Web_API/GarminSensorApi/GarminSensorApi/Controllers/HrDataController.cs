using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using GarminSensorApi.Models;
using GarminSensorApi.Models.SensorModels;
using GarminSensorApi.Utilities;

namespace GarminSensorApi.Controllers
{
    public class HrDataController : DataController<HeartRateBatch>
    {
        private readonly IRepository<HeartRateBatch> m_accelerationDataRepository;

        public HrDataController()
        {
            m_accelerationDataRepository = new Repository<HeartRateBatch>();
        }

        //public override IEnumerable<HeartRateBatch> Get()
        //{
        //    throw new System.NotImplementedException();
        //}

        [HttpGet]
        public IHttpActionResult Get(HeartRateBatch data)
        {
            throw new NotImplementedException();
        }

        public override HeartRateBatch Get(int id)
        {
            throw new System.NotImplementedException();
        }

        [HttpPost]
        [ResponseType(typeof(HeartRateBatch))]
        public override IHttpActionResult Post(HeartRateBatch data)
        {
            if (data == null)
            {
                return BadRequest("Invalid passed data.");
            }

            if (m_accelerationDataRepository.Add(data))
            {
                return Ok();
            }

            return new InternalServerErrorResult(new HttpRequestMessage());
        }

       


        public override IHttpActionResult Put(int id, HeartRateBatch data)
        {
            throw new System.NotImplementedException();
        }

        public override IHttpActionResult Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }

    public class CevaController : DataController<Ceva>
    {
        public override IHttpActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override Ceva Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public override IHttpActionResult Post([FromBody] Ceva data)
        {
            throw new NotImplementedException();
        }

        public override IHttpActionResult Put(int id, [FromBody] Ceva data)
        {
            throw new NotImplementedException();
        }
    }

    public class Ceva : IDataTableModel
    {
        public long? Id { get; set; }

        public string S { get; set; }
    }

}
