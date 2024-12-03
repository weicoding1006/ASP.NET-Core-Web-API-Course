using ClassLibrary1;
using ClassLibrary1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using AutoMapper;
using ClassLibrary1.Dtos.Country;
using WebApplication1.Contracts;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Exceptions;
using ClassLibrary1.Dtos.QueryParameters;
using Microsoft.AspNetCore.OData.Query;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CountriesController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;
        public CountriesController(IMapper mapper,ICountriesRepository countriesRepository,ILogger<CountriesController> logger)
        {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        [Authorize]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync<GetCountryDto>();
            return Ok(countries);
        }

        [HttpGet("Paged")]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery] QueryParametersDto queryParametersDto)
        {
            // 將 DTO 的數據映射到 QueryParameters
            var queryParameters = new QueryParameters
            {
                PageNumber = queryParametersDto.PageNumber,
                PageSize = queryParametersDto.PageSize
            };
            var pagedCountriedResult = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters);
            return Ok(pagedCountriedResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);
            var countryDto = _mapper.Map<CountryDto>(country);
            return Ok(countryDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            var country = await _countriesRepository.AddAsync<CreateCountryDto,GetCountryDto>(createCountryDto);
            return CreatedAtAction(nameof(GetCountry),new {id = country.Id},country);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id,UpdateCountryDto updateCountryDto)
        {
            if(id  != updateCountryDto.Id)
            {
                return BadRequest();
            }
            try
            {
                await _countriesRepository.UpdateAsync(id,updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countriesRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exists(id);
        }
    }
}
