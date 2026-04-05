using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class InvoiceRepository(PolleriaDbContext context) : BaseRepository<Invoice>(context), IInvoiceRepository { }
