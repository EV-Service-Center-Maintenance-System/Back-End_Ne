using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace EVCenterService.Pages.Customer.Feedback
{
    [Authorize(Roles = "Customer")]
    public class CreateModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public CreateModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public OrderService OrderToReview { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "Vui lòng ch?n s? sao ?ánh giá.")]
            [Range(1, 5, ErrorMessage = "Vui lòng ch?n t? 1 ??n 5 sao.")]
            public int Rating { get; set; }

            [StringLength(500, ErrorMessage = "N?i dung không v??t quá 500 ký t?.")]
            public string? Comment { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            OrderToReview = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Service)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (OrderToReview == null)
            {
                return NotFound("Không tìm th?y ??n hàng ho?c b?n không có quy?n truy c?p.");
            }

            if (OrderToReview.Status != "Completed" && OrderToReview.Status != "Paid" && OrderToReview.Status != "TechnicianCompleted")
            {
                TempData["ErrorMessage"] = "B?n ch? có th? ?ánh giá các d?ch v? ?ã hoàn thành.";
                return RedirectToPage("/Customer/Appointments/Index");
            }

            bool hasFeedback = await _context.Feedbacks.AnyAsync(f => f.OrderId == orderId);
            if (hasFeedback)
            {
                TempData["StatusMessage"] = "B?n ?ã ?ánh giá ?on hàng này r?i.";
                return RedirectToPage("/Customer/Appointments/Index");
            }

            Input = new InputModel { OrderId = orderId };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                OrderToReview = await _context.OrderServices
                    .Include(o => o.Vehicle)
                    .Include(o => o.OrderDetails).ThenInclude(od => od.Service)
                    .FirstOrDefaultAsync(o => o.OrderId == Input.OrderId && o.UserId == userId);

                if (OrderToReview == null) return NotFound();
                return Page();
            }

            var orderExists = await _context.OrderServices.AnyAsync(o => o.OrderId == Input.OrderId && o.UserId == userId);
            if (!orderExists)
            {
                return Forbid();
            }

            var feedback = new EVCenterService.Models.Feedback
            {
                OrderId = Input.OrderId,
                UserId = userId,
                Rating = Input.Rating,
                Comment = Input.Comment,
                CreatedAt = DateTime.Now 
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "C?m ?n b?n ?ã g?i ?ánh giá!";
            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}