using System.Web.Http;
using GarminSensorApi.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace GarminSensorApi.Controllers
{
    public abstract class DataController<TEntity> : ApiController where TEntity : IDataTableModel
    {
        // GET api/values/5
        [HttpGet]
        public abstract TEntity Get(int id);

        // POST api/values
        [HttpPost]
        public abstract Task<IHttpActionResult> Post(HttpRequestMessage request);
     
    }
}
