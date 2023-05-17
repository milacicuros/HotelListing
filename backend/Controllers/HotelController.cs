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
    [Route("api/hotels")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, 
            ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllHotels() {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAll();
                var result = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetAllHotels)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("{id:int}", Name = "GetHotelById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotelById(int id) {
            try
            {
                var hotels = await _unitOfWork.Hotels.Get(hotel => hotel.Id == id,  new List<string> { "Country" });
                var result = _mapper.Map<HotelDTO>(hotels);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotelById)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<IActionResult> Create([FromBody] HotelCreationDTO hotelCreationDTO) {
            if (!ModelState.IsValid) {
                _logger.LogError($"Invalid POST attempt in {nameof(Create)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotels = _mapper.Map<Hotel>(hotelCreationDTO);
                await _unitOfWork.Hotels.Insert(hotels);
                await _unitOfWork.SaveChanges();

                return CreatedAtRoute("GetHotelById", new { id = hotels.Id }, hotels);
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

        public async Task<IActionResult> Update(int id, [FromBody] HotelUpsertionDTO hotelUpsertionDTO) {
            if (!ModelState.IsValid || id < 1) {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(Update)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotels = await _unitOfWork.Hotels.Get(hotel => hotel.Id == id);
                if (hotels == null) {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(Update)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(hotelUpsertionDTO, hotels);
                _unitOfWork.Hotels.Update(hotels);
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
            if (id < 1) {
                _logger.LogError($"Invalid DELETE attempt in {nameof(Delete)}");
                return BadRequest();
            }

            try
            {
                var hotels = await _unitOfWork.Hotels.Get(hotel => hotel.Id == id);
                if (hotels == null) {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(Delete)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOfWork.Hotels.Delete(id);
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