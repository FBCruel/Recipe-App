using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using System.Security.Claims;

namespace RecipeApp.Pages
{
    [Authorize]
    public class CommentModel : PageModel
    {
        private readonly RecipeApp.Models.RecipeAppContext _context;

        public CommentModel(RecipeApp.Models.RecipeAppContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public List<Comment> Comments { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Comments = await _context.Comments
                .Where(c => c.RecipeId == id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string _content)
        {
            var comment = new Comment();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                comment.UserId = userId;
                comment.Content = _content;
                comment.RecipeId = id;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/Recipe/Index");
        }

    }
}
