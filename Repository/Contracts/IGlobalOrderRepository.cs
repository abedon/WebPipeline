/* 
 * WebPipeline 
 * https://github.com/abedon/WebPipeline 
 * 
 * Copyright (c) 2017 Dong Chen
 * Licensed under the Apache 2.0 License. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface IGlobalOrderRepository
	{
		List<PAPSwitchOrder> GetNewPAPSwitchOrders();
		List<PAPSwitchOrder> GetOrdersByBiller(int billerID);

		int GetOrderStatus(int orderId);

		void SetOrderStatus(int orderId, int statusCode);
	}

	public class PAPSwitchOrder
	{
		public int OrderId { get; set; }
		public Biller Biller { get; set; }
		public Consumer Consumer { get; set; }
		public DateTime DateCreated { get; set; }
		public int StatusCode { get; set; }
	}

	public class Biller
	{
		public int BillerId { get; set; }
		public string Name { get; set; }
		public string PADAgreement { get; set; }
		public string Address { get; set; }
	}

	public class Consumer
	{
		public int ConsumerId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CreditCard_No { get; set; }
		public string CreditCard_ExpiryDate { get; set; }
	}
}
