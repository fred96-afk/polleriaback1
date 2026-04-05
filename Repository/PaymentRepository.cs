using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class PaymentRepository(PolleriaDbContext context) : BaseRepository<Payment>(context), IPaymentRepository { }
