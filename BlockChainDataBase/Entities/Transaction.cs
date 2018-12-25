namespace Repository.Entities
{
	public class Transaction
	{
		public int TransactionId { get; set; }
		public string Name { get; set; }
		public int Amount { get; set; }
		public string Recipient { get; set; }
		public string Sender { get; set; }

		public int BlockId { get; set; }
		public virtual Block Block { get; set; }
	}
}
