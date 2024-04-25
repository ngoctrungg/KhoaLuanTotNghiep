using KLTN_E.Data;
using KLTN_E.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLTN_E.Controllers
{
    public class CommentController : Controller
    {
        private readonly KltnContext _context;

        public CommentController(KltnContext context)
        {
            _context = context;
        }

        //public IActionResult Index(int page = 1, int pageSize = 10)
        //{
        //    var comments = _context.Comments
        //        .OrderByDescending(c => c.CreatedDate)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    return View(comments);
        //}

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            int totalComments = _context.Comments.Count();
            int totalPages = (int)Math.Ceiling((double)totalComments / pageSize);

            var comments = _context.Comments
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return View(comments);
        }


        [Authorize]
        public async Task<IActionResult> AddComment(string content, int productId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySettings.CLAIM_CUSTOMER_ID);
            if (userIdClaim == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }
            var userId = userIdClaim.Value;
            var user = await _context.KhachHangs.FindAsync(userId);
            var product = await _context.HangHoas.FindAsync(productId);
            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var comment = new Comment
            {
                UserId = userId,
                Name = user.HoTen,
                ProductId = productId,
                Content = content,
                CreatedDate = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "HangHoa", new { id = productId });
        }


        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound(); 
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId)
            {
                return Forbid();
            }
            try
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Comment đã được xóa thành công.";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa comment. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "Home");
            }
        }



        public async Task<IActionResult> EditComment(int commentId, string newContent)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId)
            {
                return Forbid();
            }

            comment.Content = newContent;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment đã được chỉnh sửa thành công.";
            return RedirectToAction("Index", "Home");
        }



    }
}
