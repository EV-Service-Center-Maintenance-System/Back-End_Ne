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

            [Required(ErrorMessage = "Vui l�ng ch?n s? sao ?�nh gi�.")]
            [Range(1, 5, ErrorMessage = "Vui l�ng ch?n t? 1 ??n 5 sao.")]
            public int Rating { get; set; }

            [StringLength(500, ErrorMessage = "N?i dung kh�ng v??t qu� 500 k� t?.")]
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
                return NotFound("Kh�ng t�m th?y ??n h�ng ho?c b?n kh�ng c� quy?n truy c?p.");
            }

            if (OrderToReview.Status != "Completed" && OrderToReview.Status != "Paid" && OrderToReview.Status != "TechnicianCompleted")
            {
                TempData["ErrorMessage"] = "B?n ch? c� th? ?�nh gi� c�c d?ch v? ?� ho�n th�nh.";
                return RedirectToPage("/Customer/Appointments/Index");
            }

            bool hasFeedback = await _context.Feedbacks.AnyAsync(f => f.OrderId == orderId);
            if (hasFeedback)
            {
                TempData["StatusMessage"] = "B?n ?� ?�nh gi� ?on h�ng n�y r?i.";
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

            TempData["StatusMessage"] = "C?m ?n b?n ?� g?i ?�nh gi�!";
            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}