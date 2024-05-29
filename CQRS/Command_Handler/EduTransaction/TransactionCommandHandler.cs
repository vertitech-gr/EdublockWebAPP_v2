using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduTransaction;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Command_Handler.EduTransaction
{
    public class TransactionCommandHandler : IRequestHandler<TransactionCommand, Transactions>
    {
        private readonly EduBlockDataContext _context;
        private readonly Util _util;

        public TransactionCommandHandler(EduBlockDataContext context, Util util)
        {
            _context = context;
            _util = util;
        }

        public async Task<Transactions> Handle(TransactionCommand request, CancellationToken cancellationToken)
        {
            CommanUser employer = request.User;
            var result = new Transactions
            {
               Type = request.transactionDTO.Type,
               Status = request.transactionDTO.Status,
               Amount = request.transactionDTO.Amount,
               RefrenceId = request.transactionDTO.RefrenceId,
               PaymentID = request.transactionDTO.PaymentID,
               UserProfileID = employer.UserProfile.Id,
            };
            var subject = "Transaction Details - EduBlock";
            var tableRows = "";
                tableRows += $@"
                <tr>
                    <td>{request.transactionDTO.PaymentID}</td>
                    <td>{ DateTime.Now }</td>
                    <td>${request.transactionDTO.Amount:F2}</td>
                    <td>{request.transactionDTO.Status}</td>
                    <td>{request.transactionDTO.Type}</td>
                </tr>";
            var content = $@"
                            <p>Dear Customer,</p>
                            <p>Please find your transaction details below:</p>
                            <table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>
                                <thead>
                                    <tr>
                                        <th>Transaction ID</th>
                                        <th>Date</th>
                                        <th>Amount</th>
                                        <th>Status</th>
                                        <th>Mode</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {tableRows}
                                </tbody>
                            </table>
                            <br>
                            <p>Thank you for using EduBlock!</p>
                            ";
            _util.SendSimpleMessage(subject, employer.Email, content, null, null);
            _context.Transaction.Add(result);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving AvailableSubscription: {ex.Message}");
                throw;
            }
            return result;
        }
    }
}