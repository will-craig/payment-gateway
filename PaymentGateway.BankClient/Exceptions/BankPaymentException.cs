using System.Net;

namespace PaymentGateway.BankClient.Exceptions;

public class BankPaymentException(string message) : Exception(message);