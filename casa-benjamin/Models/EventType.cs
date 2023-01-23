namespace casa_benjamin.Models
{
    public enum EventType
    {
        MovedBed = 1,
        CanceledOrder = 2,
        RemovedOrderItem = 3,

        /// <summary>
        /// If a user paid in cash upon checkout we list it in the register flow
        /// </summary>
        CashRegisterAddFromCheckOut = 4,

        CashRegisterAddFromEmployee = 5,
        CashRegisterSubstractFromEmployee = 6,

        CashRegisterReset = 7,
        CashRegisterAddFromOrder = 8,

        /// <summary>
        /// It's possible to return to the checkout bill and change 
        /// the amount of cash paid by the user
        /// </summary>
        CashRegisterAddFromCheckOutUpdate = 9,

        /// <summary>
        /// Deposits from checkout
        /// </summary>
        CashRegisterAddPrePayment = 10,       
        CashRegisterRemovePrePayment = 11,
        CashRegisterUpdatePrePayment = 12,

        CashRegisterAddExpense = 13,
        CashRegisterAddIncome =14
    }
}