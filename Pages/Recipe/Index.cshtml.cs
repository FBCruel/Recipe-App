﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;

namespace RecipeApp.Pages.Recipe
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RecipeApp.Models.RecipeAppContext _context;

        public IndexModel(RecipeApp.Models.RecipeAppContext context)
        {
            _context = context;
        }

        public IList<Models.Recipe> Recipe { get; set; } = default!;

        public string userId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public List<double> RecipeAverageRatingsList { get; set; } = default!;

        public async Task CalculateRatings()
        {
            var AllRecipes = await (from re in _context.Recipes
                                    select re).ToListAsync();
            var AllRatings = await (from ra in _context.Ratings
                                    select ra).ToListAsync();

            RecipeAverageRatingsList = new List<Double>();

            foreach (var item in AllRecipes)
            {
                List<int> reciperates = await (from x in _context.Ratings
                                               where x.RecipeId == item.Id
                                               select x.Value).ToListAsync();
                if (reciperates.Count >= 1)
                {
                    double avg = CalculateRate.GetRating(reciperates);
                    avg = Math.Round(avg, 1);
                    RecipeAverageRatingsList.Add(avg);
                }
                else
                {
                    RecipeAverageRatingsList.Add(0.0);
                }
            }

        }

        public async Task OnGetAsync()
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_context.Recipes != null)
            {
                if (!string.IsNullOrEmpty(SearchString))
                {
                    Recipe = await _context.Recipes.Where(s => s.Title.Contains(SearchString)).ToListAsync();
                }
                else
                {
                    Recipe = await _context.Recipes.ToListAsync();
                    await CalculateRatings();
                }

            }
        }
    }
}
