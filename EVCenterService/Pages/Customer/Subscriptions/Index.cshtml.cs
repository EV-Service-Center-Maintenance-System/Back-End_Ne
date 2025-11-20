using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 

namespace EVCenterService.Pages.Customer.Subscriptions
{
    [Authorize(Roles = "Customer")]
    public class IndexModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public IndexModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public List<SubscriptionPlan> Plans { get; set; } = new();
        public bool UserHasActivePlan { get; set; }
        public string ActivePlanName { get; set; } = "";
        public DateTime ActivePlanEndDate { get; set; }
        public Guid? ActivePlanId { get; set; }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task OnGetAsync()
        {
            var userId = GetUserId();

            Plans = await _context.SubscriptionPlans.Where(p => p.IsActive == true).ToListAsync();

            var activeSubscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.Status == "active" &&
                                          s.EndDate >= DateTime.Now);

            if (activeSubscription != null)
            {
                UserHasActivePlan = true;
                ActivePlanName = activeSubscription.Plan.Name;
                ActivePlanEndDate = activeSubscription.EndDate;
                ActivePlanId = activeSubscription.PlanId;
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid planId)
        {
            var userId = GetUserId();
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return NotFound();

            var subscription = new Subscription
            {
                SubscriptionId = Guid.NewGuid(),
                UserId = userId,
                PlanId = plan.PlanId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(plan.DurationDays),
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            _context.Subscriptions.Add(subscription);

            var invoice = new Invoice
            {
                SubscriptionId = subscription.SubscriptionId,
                OrderId = null,
                Amount = plan.PriceVnd,
                Status = "Unpaid",
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
            };
            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Customer/Invoices/Details", new { id = invoice.InvoiceId });
        }
    }
}