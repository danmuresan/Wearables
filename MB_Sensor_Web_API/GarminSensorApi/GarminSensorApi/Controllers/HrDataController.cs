using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GarminSensorApi.Models.SensorModels;
using GarminSensorApi.Utilities;
using System.Threading.Tasks;

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
        public async override Task<IHttpActionResult> Post(HttpRequestMessage request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            // TODO:
            return Ok();
        }


        public IHttpActionResult Put(int id, HeartRateBatch data)
        {
            throw new System.NotImplementedException();
        }

        public IHttpActionResult Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
    
}
