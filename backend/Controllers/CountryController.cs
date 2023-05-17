using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend.Data;
using backend.DataAccess.IRepository;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, 
        ILogger<CountryController> logger,
        IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries() {
            try
            {
                var countries = await _unitOfWork.Countries.GetAll();
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetAllCountries)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("{id:int}", Name = "GetCountryById")]
        public async Task<IActionResult> GetCountryById(int id) {
            try
            {
                var country = await _unitOfWork.Countries.Get(x => x.Id == id, new List<string> { "Hotels" });
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountryById)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CountryCreationDTO countryCreationDTO) {
            if (!ModelState.IsValid) {
                _logger.LogError($"Invalid POST attempt in {nameof(Create)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = _mapper.Map<Country>(countryCreationDTO);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.SaveChanges();

                return CreatedAtRoute("GetCountryById", new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Create)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Update(int id, [FromBody] CountryUpsertionDTO countryUpsertionDTO) {
            if (!ModelState.IsValid || id < 1) {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(Update)}");
                return BadRequest(ModelState);
            }

            try
            {
                var countries = await _unitOfWork.Countries.Get(country => country.Id == id);
                if (countries == null) {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(Update)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(countryUpsertionDTO, countries);
                _unitOfWork.Countries.Update(countries);
                await _unitOfWork.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Update)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]            
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Delete(int id) {
            if(id < 1) {
                _logger.LogError($"Invalid DELETE attempt in {nameof(Delete)}");
                return BadRequest();
            }

            try
            {
                var countries = await _unitOfWork.Countries.Get(country => country.Id == id);
                if (countries == null) {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(Delete)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOfWork.Countries.Delete(id);
                await _unitOfWork.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Delete)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}