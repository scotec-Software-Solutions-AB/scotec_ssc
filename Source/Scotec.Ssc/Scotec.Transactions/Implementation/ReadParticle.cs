namespace Scotec.Transactions
{
    /// <summary>
    ///     Summary description for ReadParticle.
    /// </summary>
    public class ReadParticle : TransactionParticle
    {
        protected override bool SupportsMode(EParticleMode mode)
        {
            return mode == EParticleMode.Read;
        }


        protected override bool Commit()
        {
            return true;
        }


        protected override bool Rollback()
        {
            return true;
        }
    }
}