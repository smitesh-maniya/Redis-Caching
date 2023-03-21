using CachingWithRedisDemo.Models;
using CachingWithRedisDemo.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CachingWithRedisDemo.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [ODataRoutePrefix("Student")]
    public class StudentController: ODataController
    {
        public IStudentService<Student> Student { get; }
        public IDistributedCache _distributedCache;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService<Student> student, IDistributedCache distributedCache, ILogger<StudentController> logger) 
        {
            Student = student;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery()]
        public async Task<ActionResult<List<Student>>> Get()
        {
            _logger.LogInformation("Get student api...");
            var students = await Student.GetAll();

            //var tomorrow = DateTime.Now.Date.AddDays(1);
            //var totalSeconds = tomorrow.Subtract(DateTime.Now).TotalSeconds;

            //var distributedCacheEntryOption = new DistributedCacheEntryOptions();
            //distributedCacheEntryOption.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180);
            //distributedCacheEntryOption.SlidingExpiration = null; // if any appliction not using then remove

            //var jsonData = JsonConvert.SerializeObject(students);
            //var r = HttpContext.Items["cacheKey"].ToString();
            //await _distributedCache.SetStringAsync(r, jsonData, distributedCacheEntryOption);
            return Ok(students);
        }

        [ODataRoute("({id})")]
        [EnableQuery()]
        public async Task<ActionResult<List<Student>>> Get([FromODataUri] int id)
        {
            _logger.LogInformation("Get student odata api...");
            var students = await Student.GetById(id);

            //var tomorrow = DateTime.Now.Date.AddDays(1);
            //var totalSeconds = tomorrow.Subtract(DateTime.Now).TotalSeconds;

            //var distributedCacheEntryOption = new DistributedCacheEntryOptions();
            //distributedCacheEntryOption.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(totalSeconds);
            //distributedCacheEntryOption.SlidingExpiration = null; // if any appliction not using then remove

            //var jsonData = JsonConvert.SerializeObject(students);
            //var r = HttpContext.Items["cacheKey"].ToString();
            //await _distributedCache.SetStringAsync(r, jsonData, distributedCacheEntryOption);

            return Ok(students);
        }


    }
}
