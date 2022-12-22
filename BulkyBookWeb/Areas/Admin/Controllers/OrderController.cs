using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderVM OrderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(oh => oh.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetails.GetAll(od => od.OrderId == orderId, includeProperties: "Product")
			};

            return View(OrderVM);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails(int orderId)
		{
			var orderHEaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
			orderHEaderFromDb.Name = OrderVM.OrderHeader.Name;
			orderHEaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHEaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHEaderFromDb.City = OrderVM.OrderHeader.City;
			orderHEaderFromDb.State = OrderVM.OrderHeader.State;
			orderHEaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
			if (OrderVM.OrderHeader.Carrier != null)
			{
				orderHEaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}
			if (OrderVM.OrderHeader.TrackingNumber != null)
			{
				orderHEaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderHEaderFromDb);
			_unitOfWork.Save();
			TempData["Success"] = "Order Details Updated successfully";


			return RedirectToAction("Details", "Order", new { orderId = orderHEaderFromDb.Id }); 
		}


		#region API CALLS

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;
			if(User.IsInRole(SD.Role_User_Admin) || User.IsInRole(SD.Role_User_Employee))
			{
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				orderHeaders = _unitOfWork.OrderHeader.GetAll(oh => oh.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
			}

			

			switch (status)
			{
                case "pending":
                    orderHeaders =  orderHeaders.Where(oh => oh.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
				case "inprocess":
                    orderHeaders = orderHeaders.Where(oh => oh.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(oh => oh.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(oh => oh.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

		#endregion
	}
}
