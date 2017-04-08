using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
	public class GlobalOrderRepository : IGlobalOrderRepository
	{
		List<PAPSwitchOrder> PAPSwitchOrders;

		public GlobalOrderRepository()
		{
			PAPSwitchOrders = new List<PAPSwitchOrder>
			{
				new PAPSwitchOrder{OrderId=1, Biller=new Biller{BillerId=1, Name="Biller #1"}, Consumer = new Consumer{ConsumerId=1, FirstName="Consumer #1"}, DateCreated = DateTime.Now, StatusCode=0},
				new PAPSwitchOrder{OrderId=2, Biller=new Biller{BillerId=2, Name="Biller #2"}, Consumer = new Consumer{ConsumerId=2, FirstName="Consumer #2"}, DateCreated = DateTime.Now, StatusCode=0},
				new PAPSwitchOrder{OrderId=3, Biller=new Biller{BillerId=3, Name="Biller #3"}, Consumer = new Consumer{ConsumerId=3, FirstName="Consumer #3"}, DateCreated = DateTime.Now, StatusCode=0}
			};
		}

		public List<PAPSwitchOrder> GetNewPAPSwitchOrders()
		{
			return PAPSwitchOrders;
		}

		public List<PAPSwitchOrder> GetOrdersByBiller(int billerId)
		{
			return PAPSwitchOrders.Where(x => x.Biller.BillerId == billerId).Select(x => x).ToList();

		}

		public int GetOrderStatus(int orderId)
		{
			var order = PAPSwitchOrders.Where(x => x.OrderId == orderId).SingleOrDefault();
			if (order != null)
				return order.StatusCode;
			else
				return -1;
		}

		public void SetOrderStatus(int orderId, int statusCode)
		{
			var order = PAPSwitchOrders.Where(x => x.OrderId == orderId).SingleOrDefault();
			if (order != null)
				order.StatusCode = statusCode;
		}
	}
}
