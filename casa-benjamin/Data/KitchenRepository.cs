using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper.Contrib.Extensions;
using System.Transactions;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Restaurant.Order.Entities;

namespace casa_benjamin.Data
{
    public class KitchenRepository
    {
        GenericRepository genericRepository = new GenericRepository();

        public int InsertOrder(Order order, List<OrderItems> orderItems)
        {
            int orderId = -1;
            using (var transactionScope = new TransactionScope())
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
                {
                    con.Open();
                    orderId = (int)con.Insert(order);

                    //Could be empty in a case of splited bill
                    if(orderItems.Count > 0)
                    {
                        foreach (var item in orderItems)
                        {
                            item.order_id = orderId;
                            item.user_id = order.user_id;
                        }

                        con.Insert(orderItems);
                    }

                }

                transactionScope.Complete();
            }

            return orderId;
        }

    }
}