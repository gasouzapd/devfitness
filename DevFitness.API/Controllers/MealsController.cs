﻿using AutoMapper;
using DevFitness.API.Core.Entities;
using DevFitness.API.Models.InputModels;
using DevFitness.API.Models.ViewModels;
using DevFitness.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevFitness.API.Controllers
{
    [Route("api/users/{userId}/meals")]
    public class MealsController : ControllerBase
    {
        private readonly DevFitnessDbContext _dbContext;
        private readonly IMapper _mapper;
        public MealsController(DevFitnessDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll(int userId)
        {
            var allMeals = _dbContext.Meals.Where(m => m.UserId == userId && m.Active);

            var allMealsViewModels = allMeals
                .Select(m => new MealViewModel(m.Id, m.Description, m.Calories, m.Date));

            return Ok(allMealsViewModels);
        }

        [HttpGet("{mealId}")]
        public IActionResult Get(int userId, int mealId)
        {
            var meal = _dbContext.Meals.SingleOrDefault(m => m.Id == mealId && m.UserId == userId);

            if (meal == null)
                return NotFound();

            var mealViewModel = _mapper.Map<MealViewModel>(meal);

            return Ok(mealViewModel);
        }

        [HttpPost]
        public IActionResult Post(int userId, [FromBody] CreateMealInputModel inputModel)
        {
            var meal = _mapper.Map<Meal>(inputModel);

            _dbContext.Meals.Add(meal);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { userId = userId, mealId = meal.Id }, inputModel);
        }

        [HttpPut("{mealId}")]
        public IActionResult Put(int userId, int mealId, [FromBody] UpdateMealInputModel inputModel)
        {
            var meal = _dbContext.Meals.SingleOrDefault(m => m.UserId == userId && m.Id == mealId);

            if (meal == null)
                return NotFound();

            meal.Update(inputModel.Description, inputModel.Calories, inputModel.Date);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{mealId}")]
        public IActionResult Delete(int userId, int mealId)
        {
            var meal = _dbContext.Meals.SingleOrDefault(m => m.UserId == userId && m.Id == mealId);

            if (meal == null)
                return NotFound();

            meal.Deactivate();

            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
