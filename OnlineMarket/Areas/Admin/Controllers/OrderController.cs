using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using OnlineMarket.Models.ViewModels;
using OnlineMarket.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Braintree;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBrainTreeGate _brainTreeGate;
        private readonly IOrderHeaderRepository _orderHeaderRepository;

        [BindProperty]
        public OrderDetailsViewModel orderDetailsViewModel { get; set; }

        public OrderController(IUnitOfWork unitOfWork, IBrainTreeGate brainTreeGate, IOrderHeaderRepository orderHeaderRepository)
        {
            _unitOfWork = unitOfWork;
            _brainTreeGate = brainTreeGate;
            _orderHeaderRepository = orderHeaderRepository;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Details(int id)
        {
            orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id,
                                                includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == id, includeProperties: "Product")

            };

            return View(orderDetailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderDetailsViewModel.OrderHeader.Id,
                                                includeProperties: "ApplicationUser");

            return RedirectToAction("Details", "Order", new { id = orderHeader.Id });
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_SuperAdmin)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == id);

            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.Save();

            TempData[SD.Success] = $"Order is in process! Order Id:{orderHeader.Id}!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_SuperAdmin)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == orderDetailsViewModel.OrderHeader.Id);

            orderHeader.TrackingNumber = orderDetailsViewModel.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderDetailsViewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.Save();

            TempData[SD.Success] = $"Order shipped successfully! Order Id:{orderHeader.Id}!";

            return RedirectToAction("Index");
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_SuperAdmin)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == id);

            var gateway = _brainTreeGate.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refund
                Result<Transaction> resultvoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = SD.StatusRefunded;

            _unitOfWork.Save();

            TempData[SD.Success] = $"Transaction Id:{orderHeader.TransactionId} - Refunded successfully";

            return RedirectToAction("Index");
        }

        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderModel = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderDetailsViewModel.OrderHeader.Id);
            orderHeaderModel.Name = orderDetailsViewModel.OrderHeader.Name;
            orderHeaderModel.PhoneNumber = orderDetailsViewModel.OrderHeader.PhoneNumber;
            orderHeaderModel.Address = orderDetailsViewModel.OrderHeader.Address;
            orderHeaderModel.City = orderDetailsViewModel.OrderHeader.City;
            orderHeaderModel.State = orderDetailsViewModel.OrderHeader.State;
            orderHeaderModel.PostalCode = orderDetailsViewModel.OrderHeader.PostalCode;
            if (orderDetailsViewModel.OrderHeader.Carrier != null)
            {
                orderHeaderModel.Carrier = orderDetailsViewModel.OrderHeader.Carrier;
            }
            if (orderDetailsViewModel.OrderHeader.TrackingNumber != null)
            {
                orderHeaderModel.TrackingNumber = orderDetailsViewModel.OrderHeader.TrackingNumber;
            }

            _unitOfWork.Save();
            TempData[SD.Success] = "Order Details Updated Successfully.";

            return RedirectToAction("Details", "Order", new { id = orderHeaderModel.Id });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee) || User.IsInRole(SD.Role_SuperAdmin))
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser"); //User?
            }
            else
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(i => i.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser"); //User?
            }

            switch (status)
            {
                case "pending": orderHeaderList = orderHeaderList.Where(i => i.PaymentStatus == SD.PaymentStatusDelayedPayment); break;

                case "inprocess": orderHeaderList = orderHeaderList.Where(i => 
                       i.OrderStatus == SD.StatusApproved 
                    || i.OrderStatus ==SD.StatusInProcess 
                    || i.OrderStatus == SD.StatusPending); break;

                case "completed": orderHeaderList = orderHeaderList.Where(i => i.OrderStatus == SD.StatusShipped); break;

                case "rejected": orderHeaderList = orderHeaderList.Where(i => 
                       i.OrderStatus == SD.StatusCancelled
                    || i.OrderStatus == SD.StatusRefunded 
                    || i.OrderStatus == SD.PaymentStatusRejected); break;

                default: ; break;
            }

            return Json(new { data = orderHeaderList });
        }

        #endregion
    }
}
