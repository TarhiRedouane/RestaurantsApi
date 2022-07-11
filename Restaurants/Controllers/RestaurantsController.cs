using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Restaurants.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly LinkGenerator _linkGenerator;

        public RestaurantsController(RestaurantContext context,
            LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<IEnumerable<Restaurant>> Get()
        {
            return _context.Restaurants;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Restaurant>> Get(string name)
        {
            try
            {
                var result = await _context.Restaurants.FirstOrDefaultAsync((restaurant => restaurant.Name == name));
                if (result == null) return NotFound();
                return result;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
        [HttpPost]
        public async Task<ActionResult<Restaurant>> AddRestaurant(Restaurant model)
        {
            try
            {
                var restaurant = await _context.Restaurants.FirstOrDefaultAsync(res => res.Name == model.Name);
                if (restaurant != null) return BadRequest("this restaurant name already exists");
                var location = _linkGenerator.GetPathByAction("Get", "Restaurants", new { name = model.Name });
                if (string.IsNullOrEmpty(location)) return BadRequest("could not use the current name for the restaurant");

                await _context.Restaurants.AddAsync(model);
                if (await _context.SaveChangesAsync() > 0)
                    return Created(location, model);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpPut("{name}")]
        public async Task<ActionResult<Restaurant>> Put(string name, Restaurant model)
        {
            try
            {
                var oldresto = await _context.Restaurants.FirstOrDefaultAsync(restaurant => restaurant.Name == name);
                if (oldresto == null) return BadRequest($"Restaurant not find restaurant with name {name}");

                UpdateRestaurant(oldresto, model);
                if (await _context.SaveChangesAsync() > 0)
                    return oldresto;
                return BadRequest("nothing to change ");
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        private void UpdateRestaurant(Restaurant oldresto, Restaurant model)
        {
            oldresto.Name = model.Name;
            oldresto.Location = model.Location;
            oldresto.Owner = model.Owner;
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var oldresto = await _context.Restaurants.FirstOrDefaultAsync(restaurant => restaurant.Name == name);
                if (oldresto == null) return BadRequest($"cannot  find restaurant with name {name}");

                _context.Restaurants.Remove(oldresto);

                if (await _context.SaveChangesAsync() > 0)
                    return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest("failed to delete the restaurant");
        }

    }
}