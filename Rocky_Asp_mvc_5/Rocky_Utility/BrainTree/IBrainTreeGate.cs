using Braintree;
namespace Rocky_Utility.BrainTree
{
    public interface  IBrainTreeGate
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
