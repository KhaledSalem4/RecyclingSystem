namespace BusinessLogicLayer.Services
{
    internal interface IUnitOfWork
    {
        object OrderRepository { get; }
    }
}