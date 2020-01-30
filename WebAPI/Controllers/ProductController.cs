using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI.Dtos;
using WebAPI.Filters;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _connectString;
        public ProductController(IConfiguration configuration)
        {
            _connectString = configuration.GetConnectionString("DefaultConnection");
        }
        // GET: api/Product
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            using (var conn = new SqlConnection(_connectString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var result = await conn.QueryAsync<Product>("Get_Product_All", null, null, null, System.Data.CommandType.StoredProcedure);
                return result;
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Product> Get(int id)
        {
            //throw new Exception("test handler exception setting startup");
            using (var conn = new SqlConnection(_connectString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                var result = await conn.QueryAsync<Product>("Get_Product_ByID", parameters, null, null, System.Data.CommandType.StoredProcedure);


                return result.Single();
            }
        }

        // POST: api/Product
        [HttpPost]
        [ValidateModel]
        public async Task<int> Post([FromBody] Product product)
        {
            int newId = 0;
            using (var conn = new SqlConnection(_connectString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var paramaters = new DynamicParameters();
                paramaters.Add("@sku", product.Sku);
                paramaters.Add("@price", product.Price);
                paramaters.Add("@isActive", product.IsActive);
                paramaters.Add("@imageUrl", product.ImageUrl);
                paramaters.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conn.ExecuteAsync("Create_Product", paramaters, null, null, System.Data.CommandType.StoredProcedure);

                newId = paramaters.Get<int>("@id");
            }
            return newId;
        }


        // PUT: api/Product/5
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task Put(int id, [FromBody] Product product)
        {
            using (var conn = new SqlConnection(_connectString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                paramaters.Add("@sku", product.Sku);
                paramaters.Add("@price", product.Price);
                paramaters.Add("@isActive", product.Sku);
                paramaters.Add("@imageUrl", product.ImageUrl);
                await conn.ExecuteAsync("Update_Product", paramaters, null, null, System.Data.CommandType.StoredProcedure);
            }
        }

        // PUT: api/Product/5
        [HttpGet( "paging", Name = "GetPaging")]
        public async Task<PageResult<Product>> GetPaging(string keywork, int category, int pageIndex, int pageSize)
        {
            using (var conn = new SqlConnection(_connectString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@keywork", keywork);
                parameters.Add("@category", category);
                parameters.Add("@pageIndex", pageIndex);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@totalRow", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conn.QueryAsync<Product>("Get_Product_AllPaging", parameters, null, null, System.Data.CommandType.StoredProcedure);

                var totalRow = parameters.Get<int>("@totalRow");
                var pagedResult = new PageResult<Product>
                {
                    Items = result.ToList(),
                    TotalRow = totalRow,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return pagedResult;
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task  Delete(int id)
        {
            using (var conn = new SqlConnection(_connectString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                
                await conn.ExecuteAsync("Delete_Product_ById", paramaters, null, null, System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
